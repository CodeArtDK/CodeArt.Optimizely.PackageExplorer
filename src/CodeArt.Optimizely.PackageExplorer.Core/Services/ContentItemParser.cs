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
                .Descendants("RawProperty");

            foreach (var prop in properties)
            {
                item.Properties.Add(new ContentProperty
                {
                    Name = (string?)prop.Element("Name") ?? "",
                    Type = (string?)prop.Element("Type") ?? "",
                    Value = prop.Element("Value")?.Value?.Trim(),
                    PropertyDefinitionID = int.Parse((string?) prop.Element("PropertyDefinitionID") ?? "-1" ),
                    OwnerTab = int.Parse((string?)prop.Element("OwnerTab") ?? "-1"),
                    TypeName = (string?)prop.Element("TypeName")
                });
            }

            items.Add(item);
        }

        return items;
    }

    private static Guid ParseGuid(XElement? el)
    {
        if (el == null) return Guid.Empty;
        var str = el.Value?.Trim() ?? "";
        if (Guid.TryParse(str, out var g)) return g;
        return Guid.Empty;
    }
}
