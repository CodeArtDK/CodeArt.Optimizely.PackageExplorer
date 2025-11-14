using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Xml.Linq;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public static class ContentItemParser
{
    public static List<ContentItem> ParseContentItems(XDocument doc)
    {
        var items = new List<ContentItem>();

        var transferElements = doc.Descendants("TransferContentData");

        foreach (var transfer in transferElements)
        {
            var rawContent = transfer.Element("RawContentData");
            if (rawContent is null)
                continue;

            var item = new ContentItem
            {
                //Id = ParseGuid(rawContent.Element("GUID")),
                //ContentTypeGuid = ParseGuid(rawContent.Element("ContentTypeID")),
            };
            //RawLanguageData, LanguageSettings,ContentLanguageSettings, DynamicProperties

            //ACL's

            var properties = rawContent
                .Elements("RawProperty");

            foreach (var prop in properties)
            {
                item.Properties.Add(ParseProperty(prop));
            }

            items.Add(item);
        }

        return items;
    }

    private static ContentProperty ParseProperty(XElement prop, int depth = 0)
    {
        // Prevent stack overflow from excessively deep nesting
        const int maxDepth = 100;
        if (depth > maxDepth)
        {
            throw new InvalidOperationException($"Maximum nesting depth of {maxDepth} exceeded. Possible circular reference in XML structure.");
        }

        var property = new ContentProperty
        {
            Name = (string?)prop.Element("Name") ?? "",
            Type = (string?)prop.Element("Type") ?? "",
            Value = prop.Element("Value")?.Value?.Trim(),
            PropertyDefinitionID = int.Parse((string?)prop.Element("PropertyDefinitionID") ?? "-1"),
            OwnerTab = int.Parse((string?)prop.Element("OwnerTab") ?? "-1"),
            TypeName = (string?)prop.Element("TypeName")
        };

        // Parse BlockTypeReference if it exists (for block types)
        var blockTypeRef = prop.Element("BlockTypeReference");
        if (blockTypeRef != null)
        {
            property.BlockTypeGuid = (string?)blockTypeRef.Element("GUID");
            property.BlockTypeName = (string?)blockTypeRef.Element("Name");
        }

        // Parse BlockProperties (nested properties for inline blocks)
        var blockProps = prop.Element("BlockProperties");
        if (blockProps != null && blockProps.HasElements)
        {
            property.BlockProperties = new List<ContentProperty>();
            foreach (var nestedProp in blockProps.Elements("RawProperty"))
            {
                property.BlockProperties.Add(ParseProperty(nestedProp, depth + 1));
            }
        }

        // Parse ListProperties (list items, often blocks)
        var listProps = prop.Element("ListProperties");
        if (listProps != null && listProps.HasElements)
        {
            property.ListProperties = new List<ContentProperty>();
            foreach (var listItem in listProps.Elements("RawProperty"))
            {
                property.ListProperties.Add(ParseProperty(listItem, depth + 1));
            }
        }

        return property;
    }

    private static Guid ParseGuid(XElement? el)
    {
        if (el == null) return Guid.Empty;
        var str = el.Value?.Trim() ?? "";
        if (Guid.TryParse(str, out var g)) return g;
        return Guid.Empty;
    }
}
