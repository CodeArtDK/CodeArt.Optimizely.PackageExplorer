using CodeArt.Optimizely.PackageExplorer.Core.Models;

namespace CodeArt.Optimizely.PackageExplorer.CLI.Services;

public class DisplayService
{
    public static void DisplayContentSummary(List<ContentItem> items)
    {
        Console.WriteLine($"\n=== Content Summary ===");
        Console.WriteLine($"Total items: {items.Count}");

        var byType = items.GroupBy(i => i.ContentTypeName ?? "Unknown")
                          .OrderByDescending(g => g.Count());

        Console.WriteLine($"\nContent by type:");
        foreach (var group in byType)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()}");
        }

        var byLanguage = items.GroupBy(i => i.PageLanguageBranch ?? "Unknown")
                             .OrderByDescending(g => g.Count());

        Console.WriteLine($"\nContent by language:");
        foreach (var group in byLanguage)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()}");
        }
    }

    public static void DisplayContentList(List<ContentItem> items, int limit = 50)
    {
        Console.WriteLine($"\n=== Content Items (showing {Math.Min(limit, items.Count)} of {items.Count}) ===");
        Console.WriteLine($"{"ContentLink",-15} {"Name",-40} {"Type",-30} {"Language",-10}");
        Console.WriteLine(new string('-', 105));

        foreach (var item in items.Take(limit))
        {
            var link = item.ContentLink ?? "N/A";
            var name = TruncateString(item.Name ?? "N/A", 40);
            var type = TruncateString(item.ContentTypeName ?? "N/A", 30);
            var lang = item.PageLanguageBranch ?? "N/A";

            Console.WriteLine($"{link,-15} {name,-40} {type,-30} {lang,-10}");
        }

        if (items.Count > limit)
        {
            Console.WriteLine($"\n... and {items.Count - limit} more items");
        }
    }

    public static void DisplayContentTypesSummary(List<ContentTypeDefinition> contentTypes)
    {
        Console.WriteLine($"\n=== Content Types Summary ===");
        Console.WriteLine($"Total content types: {contentTypes.Count}");

        var byGroup = contentTypes.GroupBy(ct => ct.GroupName ?? "Ungrouped")
                                 .OrderByDescending(g => g.Count());

        Console.WriteLine($"\nContent types by group:");
        foreach (var group in byGroup)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()}");
        }

        var totalProperties = contentTypes.Sum(ct => ct.Properties.Count);
        Console.WriteLine($"\nTotal properties across all types: {totalProperties}");
        Console.WriteLine($"Average properties per type: {(contentTypes.Count > 0 ? totalProperties / contentTypes.Count : 0)}");
    }

    public static void DisplayContentTypesList(List<ContentTypeDefinition> contentTypes, int limit = 50)
    {
        Console.WriteLine($"\n=== Content Types (showing {Math.Min(limit, contentTypes.Count)} of {contentTypes.Count}) ===");
        Console.WriteLine($"{"ID",-6} {"Name",-40} {"Group",-25} {"Properties",-12}");
        Console.WriteLine(new string('-', 90));

        foreach (var ct in contentTypes.Take(limit))
        {
            var id = ct.Id.ToString();
            var name = TruncateString(ct.Name, 40);
            var group = TruncateString(ct.GroupName ?? "N/A", 25);
            var propCount = ct.Properties.Count.ToString();

            Console.WriteLine($"{id,-6} {name,-40} {group,-25} {propCount,-12}");
        }

        if (contentTypes.Count > limit)
        {
            Console.WriteLine($"\n... and {contentTypes.Count - limit} more content types");
        }
    }

    public static void DisplayMediaSummary(List<ContentItem> mediaItems)
    {
        Console.WriteLine($"\n=== Media Summary ===");
        Console.WriteLine($"Total media items: {mediaItems.Count}");

        var byType = mediaItems.GroupBy(i => i.ContentTypeName ?? "Unknown")
                              .OrderByDescending(g => g.Count());

        Console.WriteLine($"\nMedia by type:");
        foreach (var group in byType)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()}");
        }
    }

    public static void DisplayMediaList(List<ContentItem> mediaItems, int limit = 50)
    {
        Console.WriteLine($"\n=== Media Items (showing {Math.Min(limit, mediaItems.Count)} of {mediaItems.Count}) ===");
        Console.WriteLine($"{"ContentLink",-15} {"Name",-50} {"Type",-30}");
        Console.WriteLine(new string('-', 105));

        foreach (var item in mediaItems.Take(limit))
        {
            var link = item.ContentLink ?? "N/A";
            var name = TruncateString(item.Name ?? "N/A", 50);
            var type = TruncateString(item.ContentTypeName ?? "N/A", 30);

            Console.WriteLine($"{link,-15} {name,-50} {type,-30}");
        }

        if (mediaItems.Count > limit)
        {
            Console.WriteLine($"\n... and {mediaItems.Count - limit} more items");
        }
    }

    public static void DisplayCategoriesSummary(List<CategoryDefinition> categories)
    {
        Console.WriteLine($"\n=== Categories Summary ===");
        Console.WriteLine($"Total categories: {categories.Count}");

        var topLevel = categories.Count(c => c.ParentId == null || c.ParentId == 0);
        var nested = categories.Count - topLevel;

        Console.WriteLine($"Top-level categories: {topLevel}");
        Console.WriteLine($"Nested categories: {nested}");
    }

    public static void DisplayCategoriesList(List<CategoryDefinition> categories, int limit = 50)
    {
        Console.WriteLine($"\n=== Categories (showing {Math.Min(limit, categories.Count)} of {categories.Count}) ===");
        Console.WriteLine($"{"ID",-6} {"Name",-40} {"Parent ID",-12} {"Selectable",-12}");
        Console.WriteLine(new string('-', 80));

        foreach (var cat in categories.Take(limit))
        {
            var id = cat.Id.ToString();
            var name = TruncateString(cat.Name ?? "N/A", 40);
            var parentId = (cat.ParentId ?? 0).ToString();
            var selectable = (cat.Selectable ?? false).ToString();

            Console.WriteLine($"{id,-6} {name,-40} {parentId,-12} {selectable,-12}");
        }

        if (categories.Count > limit)
        {
            Console.WriteLine($"\n... and {categories.Count - limit} more categories");
        }
    }

    public static void DisplayAudiencesSummary(List<VisitorGroup> audiences)
    {
        Console.WriteLine($"\n=== Audiences Summary ===");
        Console.WriteLine($"Total visitor groups: {audiences.Count}");

        var totalCriteria = audiences.Sum(a => a.Criteria?.Count ?? 0);
        Console.WriteLine($"Total criteria: {totalCriteria}");
        Console.WriteLine($"Average criteria per group: {(audiences.Count > 0 ? totalCriteria / audiences.Count : 0)}");
    }

    public static void DisplayAudiencesList(List<VisitorGroup> audiences, int limit = 50)
    {
        Console.WriteLine($"\n=== Visitor Groups/Audiences (showing {Math.Min(limit, audiences.Count)} of {audiences.Count}) ===");
        Console.WriteLine($"{"ID",-37} {"Name",-40} {"Criteria",-10}");
        Console.WriteLine(new string('-', 95));

        foreach (var audience in audiences.Take(limit))
        {
            var id = TruncateString(audience.Id.ToString(), 37);
            var name = TruncateString(audience.Name ?? "N/A", 40);
            var criteriaCount = (audience.Criteria?.Count ?? 0).ToString();

            Console.WriteLine($"{id,-37} {name,-40} {criteriaCount,-10}");
        }

        if (audiences.Count > limit)
        {
            Console.WriteLine($"\n... and {audiences.Count - limit} more visitor groups");
        }
    }

    public static void DisplayAvailableProperties(List<ContentItem> items)
    {
        var allProperties = items
            .SelectMany(i => i.Properties)
            .Select(p => p.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        Console.WriteLine($"\n=== Available Properties ({allProperties.Count}) ===");

        var columns = 3;
        var columnWidth = 30;

        for (int i = 0; i < allProperties.Count; i += columns)
        {
            for (int col = 0; col < columns; col++)
            {
                var index = i + col;
                if (index < allProperties.Count)
                {
                    var prop = TruncateString(allProperties[index], columnWidth - 2);
                    Console.Write(prop.PadRight(columnWidth));
                }
            }
            Console.WriteLine();
        }
    }

    private static string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - 3) + "...";
    }
}
