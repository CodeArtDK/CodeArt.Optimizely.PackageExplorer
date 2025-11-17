using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

/// <summary>
/// Streaming XML parser optimized for large package files.
/// Avoids loading entire XML into XDocument to reduce memory usage.
/// </summary>
public class StreamingPackageParser
{
    /// <summary>
    /// Parse content items from XML using streaming approach.
    /// This is memory-efficient for very large files.
    /// </summary>
    public static IEnumerable<ContentItem> StreamContentItems(ZipArchive zipArchive)
    {
        var entry = zipArchive.Entries.FirstOrDefault(e => 
            e.FullName.Equals("epix.xml", StringComparison.OrdinalIgnoreCase));
        
        if (entry == null)
            yield break;

        using var stream = entry.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
        {
            CheckCharacters = false,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = true
        });

        ContentItem? currentItem = null;
        ContentProperty? currentProperty = null;
        string? currentElement = null;

        while (xmlReader.Read())
        {
            switch (xmlReader.NodeType)
            {
                case XmlNodeType.Element:
                    currentElement = xmlReader.Name;
                    
                    if (xmlReader.Name == "TransferContentData")
                    {
                        currentItem = new ContentItem();
                    }
                    else if (xmlReader.Name == "RawProperty" && currentItem != null)
                    {
                        currentProperty = new ContentProperty();
                    }
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    if (currentProperty != null && currentElement != null)
                    {
                        var value = xmlReader.Value?.Trim();
                        switch (currentElement)
                        {
                            case "Name":
                                currentProperty.Name = value ?? "";
                                break;
                            case "Type":
                                currentProperty.Type = value ?? "";
                                break;
                            case "Value":
                                currentProperty.Value = value;
                                break;
                            case "TypeName":
                                currentProperty.TypeName = value;
                                break;
                            case "PropertyDefinitionID":
                                currentProperty.PropertyDefinitionID = int.TryParse(value, out var pdid) ? pdid : -1;
                                break;
                            case "OwnerTab":
                                currentProperty.OwnerTab = int.TryParse(value, out var tab) ? tab : -1;
                                break;
                        }
                    }
                    break;

                case XmlNodeType.EndElement:
                    if (xmlReader.Name == "RawProperty" && currentProperty != null && currentItem != null)
                    {
                        currentItem.Properties.Add(currentProperty);
                        currentProperty = null;
                    }
                    else if (xmlReader.Name == "TransferContentData" && currentItem != null)
                    {
                        yield return currentItem;
                        currentItem = null;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Count content items without loading them all into memory.
    /// </summary>
    public static int CountContentItems(ZipArchive zipArchive)
    {
        var entry = zipArchive.Entries.FirstOrDefault(e => 
            e.FullName.Equals("epix.xml", StringComparison.OrdinalIgnoreCase));
        
        if (entry == null)
            return 0;

        int count = 0;
        using var stream = entry.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
        {
            CheckCharacters = false,
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreWhitespace = true
        });

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element && 
                xmlReader.Name == "TransferContentData")
            {
                count++;
            }
        }

        return count;
    }
}
