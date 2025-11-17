using CodeArt.Optimizely.PackageExplorer.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.Json;

namespace CodeArt.Optimizely.PackageExplorer.CLI.Services;

public class ExportService
{
    public static void ExportContentToCsv(List<ContentItem> items, List<string> properties, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Write headers
        foreach (var prop in properties)
        {
            csv.WriteField(prop);
        }
        csv.NextRecord();

        // Write data
        foreach (var item in items)
        {
            foreach (var prop in properties)
            {
                var value = item.TryGetProperty(prop) ?? string.Empty;
                csv.WriteField(value);
            }
            csv.NextRecord();
        }

        Console.WriteLine($"Exported {items.Count} items to {outputPath}");
    }

    public static void ExportContentToJson(List<ContentItem> items, List<string> properties, string outputPath)
    {
        var exportData = items.Select(item =>
        {
            var dict = new Dictionary<string, string>();
            foreach (var prop in properties)
            {
                dict[prop] = item.TryGetProperty(prop) ?? string.Empty;
            }
            return dict;
        }).ToList();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(exportData, options);
        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Exported {items.Count} items to {outputPath}");
    }

    public static void ExportContentTypesToCsv(List<ContentTypeDefinition> contentTypes, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Write headers
        csv.WriteField("Id");
        csv.WriteField("Guid");
        csv.WriteField("Name");
        csv.WriteField("GroupName");
        csv.WriteField("Base");
        csv.WriteField("ModelTypeString");
        csv.WriteField("PropertyCount");
        csv.NextRecord();

        // Write data
        foreach (var ct in contentTypes)
        {
            csv.WriteField(ct.Id);
            csv.WriteField(ct.Guid);
            csv.WriteField(ct.Name);
            csv.WriteField(ct.GroupName ?? string.Empty);
            csv.WriteField(ct.Base ?? string.Empty);
            csv.WriteField(ct.ModelTypeString ?? string.Empty);
            csv.WriteField(ct.Properties.Count);
            csv.NextRecord();
        }

        Console.WriteLine($"Exported {contentTypes.Count} content types to {outputPath}");
    }

    public static void ExportContentTypesToJson(List<ContentTypeDefinition> contentTypes, string outputPath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(contentTypes, options);
        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Exported {contentTypes.Count} content types to {outputPath}");
    }

    public static void ExportCategoriesToCsv(List<CategoryDefinition> categories, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Write headers
        csv.WriteField("Id");
        csv.WriteField("Name");
        csv.WriteField("Description");
        csv.WriteField("ParentId");
        csv.WriteField("Selectable");
        csv.NextRecord();

        // Write data
        foreach (var cat in categories)
        {
            csv.WriteField(cat.Id);
            csv.WriteField(cat.Name ?? string.Empty);
            csv.WriteField(cat.Description ?? string.Empty);
            csv.WriteField(cat.ParentId ?? 0);
            csv.WriteField(cat.Selectable ?? false);
            csv.NextRecord();
        }

        Console.WriteLine($"Exported {categories.Count} categories to {outputPath}");
    }

    public static void ExportCategoriesToJson(List<CategoryDefinition> categories, string outputPath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(categories, options);
        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Exported {categories.Count} categories to {outputPath}");
    }
}
