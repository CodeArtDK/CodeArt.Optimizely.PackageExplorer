using System.IO.Compression;
using System.Xml.Linq;
using CodeArt.Optimizely.PackageExplorer.Core.Models;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public class PackageWriter
{
    public static Stream CreateModifiedPackage(
        Stream originalPackageStream,
        HashSet<string> deletedContentIds,
        HashSet<string> deletedContentTypeGuids,
        HashSet<int> deletedCategoryIds,
        Dictionary<string, string> modifiedContentProperties,
        Dictionary<string, string> modifiedContentTypeProperties)
    {
        var outputStream = new MemoryStream();
        
        // Reset original stream position
        originalPackageStream.Position = 0;
        
        using (var originalArchive = new ZipArchive(originalPackageStream, ZipArchiveMode.Read, leaveOpen: true))
        using (var newArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var entry in originalArchive.Entries)
            {
                // Process XML files that need filtering
                if (entry.FullName.Equals("epix.xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlDoc = LoadXmlFromEntry(entry);
                    FilterContentItems(xmlDoc, deletedContentIds);
                    ApplyContentPropertyModifications(xmlDoc, modifiedContentProperties);
                    WriteXmlToArchive(newArchive, "epix.xml", xmlDoc);
                }
                else if (entry.FullName.Equals("epiDefinition.xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlDoc = LoadXmlFromEntry(entry);
                    FilterContentTypes(xmlDoc, deletedContentTypeGuids);
                    FilterCategories(xmlDoc, deletedCategoryIds);
                    ApplyContentTypePropertyModifications(xmlDoc, modifiedContentTypeProperties);
                    WriteXmlToArchive(newArchive, "epiDefinition.xml", xmlDoc);
                }
                else
                {
                    // Copy other files as-is (blobs, etc.)
                    // TODO: Skip blobs that belong to deleted media items
                    CopyEntry(entry, newArchive);
                }
            }
        }
        
        outputStream.Position = 0;
        return outputStream;
    }
    
    private static XDocument LoadXmlFromEntry(ZipArchiveEntry entry)
    {
        using var stream = entry.Open();
        return XDocument.Load(stream);
    }
    
    private static void WriteXmlToArchive(ZipArchive archive, string fileName, XDocument doc)
    {
        var entry = archive.CreateEntry(fileName);
        using var stream = entry.Open();
        doc.Save(stream);
    }
    
    private static void CopyEntry(ZipArchiveEntry sourceEntry, ZipArchive targetArchive)
    {
        var targetEntry = targetArchive.CreateEntry(sourceEntry.FullName);
        using var sourceStream = sourceEntry.Open();
        using var targetStream = targetEntry.Open();
        sourceStream.CopyTo(targetStream);
    }
    
    private static void FilterContentItems(XDocument doc, HashSet<string> deletedContentIds)
    {
        if (deletedContentIds.Count == 0) return;
        
        var transferElements = doc.Descendants("TransferContentData").ToList();
        
        foreach (var transfer in transferElements)
        {
            var rawContent = transfer.Element("RawContentData");
            if (rawContent == null) continue;
            
            // Get the PageLink to identify the content item
            var pageLinkProp = rawContent.Descendants("RawProperty")
                .FirstOrDefault(p => p.Element("Name")?.Value == "PageLink");
            
            if (pageLinkProp != null)
            {
                var pageLink = pageLinkProp.Element("Value")?.Value?.Replace("[", "").Replace("]", "").Trim();
                if (pageLink != null && deletedContentIds.Contains(pageLink))
                {
                    transfer.Remove();
                }
            }
        }
    }
    
    private static void FilterContentTypes(XDocument doc, HashSet<string> deletedContentTypeGuids)
    {
        if (deletedContentTypeGuids.Count == 0) return;
        
        var contentTypeElements = doc
            .Descendants("ArrayOfContentTypeTransferObject")
            .Descendants("ContentTypeTransferObject")
            .ToList();
        
        foreach (var type in contentTypeElements)
        {
            var guid = type.Element("GUID")?.Value;
            if (guid != null && deletedContentTypeGuids.Contains(guid))
            {
                type.Remove();
            }
        }
    }
    
    private static void FilterCategories(XDocument doc, HashSet<int> deletedCategoryIds)
    {
        if (deletedCategoryIds.Count == 0) return;
        
        var categoryElements = doc
            .Descendants("ArrayOfCategory")
            .Descendants("Category")
            .ToList();
        
        foreach (var category in categoryElements)
        {
            var idStr = category.Element("ID")?.Value;
            if (idStr != null && int.TryParse(idStr, out var id) && deletedCategoryIds.Contains(id))
            {
                category.Remove();
            }
        }
    }
    
    private static void ApplyContentPropertyModifications(XDocument doc, Dictionary<string, string> modifications)
    {
        if (modifications.Count == 0) return;
        
        var transferElements = doc.Descendants("TransferContentData").ToList();
        
        foreach (var transfer in transferElements)
        {
            var rawContent = transfer.Element("RawContentData");
            if (rawContent == null) continue;
            
            // Get the PageLink to identify the content item
            var pageLinkProp = rawContent.Descendants("RawProperty")
                .FirstOrDefault(p => p.Element("Name")?.Value == "PageLink");
            
            if (pageLinkProp != null)
            {
                var pageLink = pageLinkProp.Element("Value")?.Value?.Replace("[", "").Replace("]", "").Trim();
                if (pageLink != null)
                {
                    // Find and update modified properties for this content item
                    var properties = rawContent.Descendants("RawProperty").ToList();
                    foreach (var prop in properties)
                    {
                        var propName = prop.Element("Name")?.Value;
                        if (propName != null)
                        {
                            var key = $"{pageLink}|{propName}";
                            if (modifications.TryGetValue(key, out var newValue))
                            {
                                var valueElement = prop.Element("Value");
                                if (valueElement != null)
                                {
                                    valueElement.Value = newValue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    private static void ApplyContentTypePropertyModifications(XDocument doc, Dictionary<string, string> modifications)
    {
        if (modifications.Count == 0) return;
        
        var contentTypeElements = doc
            .Descendants("ArrayOfContentTypeTransferObject")
            .Descendants("ContentTypeTransferObject")
            .ToList();
        
        foreach (var type in contentTypeElements)
        {
            var typeGuid = type.Element("GUID")?.Value;
            if (typeGuid != null)
            {
                // Find property definitions within this content type
                var propertyElements = type
                    .Descendants("PropertyDefinitionTransferObject")
                    .ToList();
                
                foreach (var prop in propertyElements)
                {
                    var propName = prop.Element("Name")?.Value;
                    if (propName != null)
                    {
                        // Check for modifications to this property's fields
                        foreach (var fieldName in new[] { "EditCaption", "IsRequired", "IsSearchable", "IsLocalizable" })
                        {
                            var key = $"{typeGuid}|{propName}|{fieldName}";
                            if (modifications.TryGetValue(key, out var newValue))
                            {
                                var fieldElement = prop.Element(fieldName);
                                if (fieldElement != null)
                                {
                                    fieldElement.Value = newValue;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
