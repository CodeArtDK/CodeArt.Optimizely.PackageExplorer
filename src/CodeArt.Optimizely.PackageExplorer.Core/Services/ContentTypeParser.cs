using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Globalization;
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
                GroupName = (string?)type.Element("GroupName"),
                Base = (string?)type.Element("Base"),
                ModelTypeString = (string?)type.Element("ModelTypeString"),
                Created = (!string.IsNullOrEmpty(type.Element("Created")?.Value)) ? DateTime.Parse(type.Element("Created").Value, CultureInfo.InvariantCulture) : null,
                Saved = (!string.IsNullOrEmpty(type.Element("Saved")?.Value)) ? DateTime.Parse(type.Element("Saved").Value, CultureInfo.InvariantCulture) : null
            };

            var props = type.Element("PropertyDefinitions")?.Elements("PropertyDefinition") ?? Enumerable.Empty<XElement>();
            foreach (var prop in props)
            {
                ct.Properties.Add(new ContentPropertyDefinition
                {
                    Name = (string?)prop.Element("Name") ?? "",
                    Type = (string?)prop.Element("Type")?.Element("Name") ?? "",
                    DataType = (string?)prop.Element("Type")?.Element("DataType")?.Value,
                    TabId = (int?)prop.Element("Tab")?.Element("ID"),
                    IsRequired = (bool?)prop.Element("Required") ?? false,
                    IsSearchable = (bool?)prop.Element("Searchable") ?? false,
                    IsLocalizable = (bool?)prop.Element("LanguageSpecific") ?? false,
                    FieldOrder = (int?)prop.Element("FieldOrder") ?? 0,
                    EditCaption = (string?)prop.Element("EditCaption")?.Value,
                    DisplayEditUI = (bool?)prop.Element("DisplayEditUI") ?? false,
                    ExistsOnModel = (bool?)prop.Element("ExistsOnModel") ?? false
                });
            }

            types.Add(ct);
        }

        return types;
    }

    public static List<CategoryDefinition> ParseCategories(XDocument doc)
    {
        var categories = new List<CategoryDefinition>();

        var categoryElements = doc
            .Descendants("ArrayOfCategory")
            .Descendants("Category");

        foreach (var category in categoryElements)
        {
            categories.Add(new CategoryDefinition
            {
                Id = (int)category.Element("ID"),
                Name = (string?)category.Element("Name"),
                Description = (string?)category.Element("Description"),
                ParentId = (int?)category.Element("ParentID"),
                Selectable = (bool?)category.Element("Selectable")
            });
        }

        return categories;
    }

    public static List<DisplayTemplate> ParseDisplayTemplates(XDocument doc)
    {
        var templates = new List<DisplayTemplate>();

        var templateElements = doc
            .Descendants("displaytemplates")
            .Descendants("DisplayTemplate");

        foreach (var tmpl in templateElements)
        {
            var template = new DisplayTemplate
            {
                Key = (string?)tmpl.Element("Key") ?? "",
                Name = (string?)tmpl.Element("Name") ?? "",
                ContentTypeName = (string?)tmpl.Element("ContentTypeName"),
                BaseType = (string?)tmpl.Element("BaseType"),
                NodeType = (string?)tmpl.Element("NodeType"),
                IsDefault = (bool?)tmpl.Element("IsDefault") ?? false
            };

            var settings = tmpl.Element("Settings")?.Elements("Setting");
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    var s = new DisplayTemplateSetting
                    {
                        Key = (string?)setting.Element("Key") ?? "",
                        Name = (string?)setting.Element("Name") ?? "",
                        Editor = (string?)setting.Element("Editor"),
                        SortOrder = (int?)setting.Element("SortOrder") ?? 0
                    };

                    var choices = setting.Element("Choices")?.Elements("Choice");
                    if (choices != null)
                    {
                        foreach (var choice in choices)
                        {
                            s.Choices.Add(new DisplayTemplateChoice
                            {
                                Key = (string?)choice.Element("Key") ?? "",
                                Name = (string?)choice.Element("Name") ?? "",
                                SortOrder = (int?)choice.Element("SortOrder") ?? 0
                            });
                        }
                    }

                    template.Settings.Add(s);
                }
            }

            templates.Add(template);
        }

        return templates;
    }
}
