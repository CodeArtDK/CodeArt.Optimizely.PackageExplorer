using CodeArt.Optimizely.PackageExplorer.Core.Models;
using CodeArt.Optimizely.PackageExplorer.Core.Services;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.Json;

namespace CodeArt.Optimizely.PackageExplorer.CLI.Services;

public class ExportService
{
    private const int LARGE_FILE_THRESHOLD = 10_000; // Items count threshold for streaming

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

    /// <summary>
    /// Export content using streaming approach for very large files.
    /// This is memory-efficient.
    /// </summary>
    public static void ExportContentToCsvStreaming(PackageReader reader, List<string> properties, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Write headers
        foreach (var prop in properties)
        {
            csv.WriteField(prop);
        }
        csv.NextRecord();

        int count = 0;
        // Stream and write data
        foreach (var item in reader.StreamContentItems())
        {
            foreach (var prop in properties)
            {
                var value = item.TryGetProperty(prop) ?? string.Empty;
                csv.WriteField(value);
            }
            csv.NextRecord();
            count++;

            if (count % 1000 == 0)
            {
                Console.Write($"\rExporting... {count} items processed");
            }
        }

        Console.WriteLine($"\rExported {count} items to {outputPath}");
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

    /// <summary>
    /// Export content to JSON using streaming approach for very large files.
    /// </summary>
    public static void ExportContentToJsonStreaming(PackageReader reader, List<string> properties, string outputPath)
    {
        using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        using var writer = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true });

        writer.WriteStartArray();

        int count = 0;
        foreach (var item in reader.StreamContentItems())
        {
            writer.WriteStartObject();
            foreach (var prop in properties)
            {
                var value = item.TryGetProperty(prop) ?? string.Empty;
                writer.WriteString(prop, value);
            }
            writer.WriteEndObject();
            count++;

            if (count % 1000 == 0)
            {
                Console.Write($"\rExporting... {count} items processed");
            }
        }

        writer.WriteEndArray();
        writer.Flush();

        Console.WriteLine($"\rExported {count} items to {outputPath}");
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
