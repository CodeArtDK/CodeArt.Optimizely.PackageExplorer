using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Xml.Linq;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public static class ContentTypeParser
{
    public static List<TabDefinition> ParseTabs(XDocument doc)
    {
        var tabs = new List<TabDefinition>();
        var ns = doc.Root?.GetDefaultNamespace();

        var tabElements = doc
            .Descendants("ArrayOfTabDefinition")
            .Descendants("TabDefinition");

        foreach (var tab in tabElements)
        {
            tabs.Add(new TabDefinition
            {
                Id = (int)tab.Element("ID"),
                Name = (string)tab.Element("Name"),
                DisplayName = (string?)tab.Element("DisplayName"),
                SortIndex = (int)tab.Element("SortIndex"),
                IsSystemTab = (bool)tab.Element("IsSystemTab")
            });
        }

        return tabs;
    }

    public static List<ContentTypeDefinition> ParseContentTypes(XDocument doc)
    {
        var types = new List<ContentTypeDefinition>();

        var contentTypeElements = doc
            .Descendants("ArrayOfContentTypeTransferObject")
            .Descendants("ContentTypeTransferObject");

        foreach (var type in contentTypeElements)
        {
            var ct = new ContentTypeDefinition
            {
                Id = (int)type.Element("ID"),
                Guid = Guid.Parse((string)type.Element("GUID")),
                Name = (string?)type.Element("Name") ?? "",
                Description = (string?)type.Element("Description")
            };

            var props = type.Element("Properties")?.Elements("PropertyDefinitionTransferObject") ?? Enumerable.Empty<XElement>();
            foreach (var prop in props)
            {
                ct.Properties.Add(new ContentPropertyDefinition
                {
                    Name = (string?)prop.Element("Name") ?? "",
                    Type = (string?)prop.Element("Type") ?? "",
                    TabId = (int?)prop.Element("Tab"),
                    IsRequired = (bool?)prop.Element("Required") ?? false,
                    IsSearchable = (bool?)prop.Element("Searchable") ?? false,
                    IsLocalizable = (bool?)prop.Element("LanguageSpecific") ?? false
                });
            }

            types.Add(ct);
        }

        return types;
    }
}
