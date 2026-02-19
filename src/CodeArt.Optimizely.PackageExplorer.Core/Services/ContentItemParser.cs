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

            var item = new ContentItem();

            // Only parse direct child RawProperty elements (not nested ones from BlockProperties/ListProperties)
            var properties = rawContent
                .Element("Property")?.Elements("RawProperty")
                ?? rawContent.Elements("RawProperty");

            foreach (var prop in properties)
            {
                item.Properties.Add(ParseRawProperty(prop));
            }

            items.Add(item);
        }

        return items;
    }

    private static ContentProperty ParseRawProperty(XElement prop, int depth = 0)
    {
        var property = new ContentProperty
        {
            Name = (string?)prop.Element("Name") ?? "",
            Type = (string?)prop.Element("Type") ?? "",
            Value = prop.Element("Value")?.Value?.Trim(),
            PropertyDefinitionID = int.Parse((string?)prop.Element("PropertyDefinitionID") ?? "-1"),
            OwnerTab = int.Parse((string?)prop.Element("OwnerTab") ?? "-1"),
            TypeName = (string?)prop.Element("TypeName")
        };

        // Guard against excessive nesting
        if (depth >= 10)
            return property;

        // Parse BlockTypeReference if present
        var blockTypeRef = prop.Element("BlockTypeReference");
        if (blockTypeRef != null)
        {
            property.BlockType = new BlockTypeReference
            {
                Guid = (string?)blockTypeRef.Element("GUID"),
                Name = (string?)blockTypeRef.Element("Name")
            };
        }

        // Parse nested BlockProperties
        var blockProps = prop.Element("BlockProperties")?.Elements("RawProperty");
        if (blockProps != null)
        {
            foreach (var nested in blockProps)
            {
                property.BlockProperties.Add(ParseRawProperty(nested, depth + 1));
            }
        }

        // Parse nested ListProperties
        var listProps = prop.Element("ListProperties")?.Elements("RawProperty");
        if (listProps != null)
        {
            foreach (var nested in listProps)
            {
                property.ListItems.Add(ParseRawProperty(nested, depth + 1));
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
