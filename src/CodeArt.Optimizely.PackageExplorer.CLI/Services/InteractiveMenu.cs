using CodeArt.Optimizely.PackageExplorer.Core.Models;
using CodeArt.Optimizely.PackageExplorer.Core.Services;

namespace CodeArt.Optimizely.PackageExplorer.CLI.Services;

public class InteractiveMenu
{
    private readonly PackageReader _packageReader;
    private List<ContentItem>? _contentItems;
    private List<ContentTypeDefinition>? _contentTypes;
    private List<CategoryDefinition>? _categories;
    private List<VisitorGroup>? _audiences;

    public InteractiveMenu(string packagePath)
    {
        _packageReader = new PackageReader(packagePath);
    }

    private static void WaitForKey()
    {
        if (!Console.IsInputRedirected)
        {
            WaitForKey();

        }
        else
        {
            Console.WriteLine();
        }
    }

    public void Run()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
        Console.WriteLine("   Optimizely Package Explorer - Interactive Menu");
        Console.WriteLine("═══════════════════════════════════════════════════════════\n");

        bool running = true;
        while (running)
        {
            DisplayMainMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ViewContent();
                    break;
                case "2":
                    ViewContentTypes();
                    break;
                case "3":
                    ViewMedia();
                    break;
                case "4":
                    ViewCategories();
                    break;
                case "5":
                    ViewAudiences();
                    break;
                case "6":
                    ExportContentMenu();
                    break;
                case "7":
                    ExportContentTypesMenu();
                    break;
                case "8":
                    ExportCategoriesMenu();
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("\nGoodbye!");
                    break;
                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;
            }
        }
    }

    private void DisplayMainMenu()
    {
        Console.WriteLine("\n─────────────────────────────────────────────────────────");
        Console.WriteLine("Main Menu");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.WriteLine("  1. View Content");
        Console.WriteLine("  2. View Content Types");
        Console.WriteLine("  3. View Media");
        Console.WriteLine("  4. View Categories");
        Console.WriteLine("  5. View Audiences/Visitor Groups");
        Console.WriteLine("  6. Export Content to CSV/JSON");
        Console.WriteLine("  7. Export Content Types to CSV/JSON");
        Console.WriteLine("  8. Export Categories to CSV/JSON");
        Console.WriteLine("  0. Exit");
        Console.WriteLine("─────────────────────────────────────────────────────────");
        Console.Write("Select an option: ");
    }

    private void ViewContent()
    {
        LoadContentItems();

        if (_contentItems == null || _contentItems.Count == 0)
        {
            Console.WriteLine("\nNo content items found in the package.");
            return;
        }

        DisplayService.DisplayContentSummary(_contentItems);
        DisplayService.DisplayContentList(_contentItems);

        WaitForKey();
    }

    private void ViewContentTypes()
    {
        LoadContentTypes();

        if (_contentTypes == null || _contentTypes.Count == 0)
        {
            Console.WriteLine("\nNo content types found in the package.");
            return;
        }

        DisplayService.DisplayContentTypesSummary(_contentTypes);
        DisplayService.DisplayContentTypesList(_contentTypes);

        WaitForKey();

    }

    private void ViewMedia()
    {
        LoadContentItems();

        if (_contentItems == null || _contentItems.Count == 0)
        {
            Console.WriteLine("\nNo content items found in the package.");
            return;
        }

        var mediaItems = _contentItems
            .Where(i => i.ContentTypeName?.Contains("Media", StringComparison.OrdinalIgnoreCase) == true ||
                       i.ContentTypeName?.Contains("Image", StringComparison.OrdinalIgnoreCase) == true ||
                       i.ContentTypeName?.Contains("Video", StringComparison.OrdinalIgnoreCase) == true ||
                       i.ContentTypeName?.Contains("File", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (mediaItems.Count == 0)
        {
            Console.WriteLine("\nNo media items found in the package.");
            return;
        }

        DisplayService.DisplayMediaSummary(mediaItems);
        DisplayService.DisplayMediaList(mediaItems);

        WaitForKey();

    }

    private void ViewCategories()
    {
        LoadCategories();

        if (_categories == null || _categories.Count == 0)
        {
            Console.WriteLine("\nNo categories found in the package.");
            return;
        }

        DisplayService.DisplayCategoriesSummary(_categories);
        DisplayService.DisplayCategoriesList(_categories);

        WaitForKey();

    }

    private void ViewAudiences()
    {
        LoadAudiences();

        if (_audiences == null || _audiences.Count == 0)
        {
            Console.WriteLine("\nNo audiences/visitor groups found in the package.");
            return;
        }

        DisplayService.DisplayAudiencesSummary(_audiences);
        DisplayService.DisplayAudiencesList(_audiences);

        WaitForKey();

    }

    private void ExportContentMenu()
    {
        LoadContentItems();

        if (_contentItems == null || _contentItems.Count == 0)
        {
            Console.WriteLine("\nNo content items found in the package.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        Console.WriteLine("\n─────────────────────────────────────────────────────────");
        Console.WriteLine("Export Content");
        Console.WriteLine("─────────────────────────────────────────────────────────");

        // Show available properties
        DisplayService.DisplayAvailableProperties(_contentItems);

        Console.WriteLine("\nEnter property names (comma-separated):");
        Console.WriteLine("Example: PageName,PageTypeName,PageLink,PageLanguageBranch");
        Console.Write("> ");
        var propertiesInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(propertiesInput))
        {
            Console.WriteLine("\nNo properties specified. Export cancelled.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        var properties = propertiesInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(p => p.Trim())
                                       .ToList();

        Console.WriteLine("\nSelect export format:");
        Console.WriteLine("  1. CSV");
        Console.WriteLine("  2. JSON");
        Console.Write("> ");
        var formatChoice = Console.ReadLine()?.Trim();

        Console.Write("\nEnter output file path: ");
        var outputPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine("\nNo output path specified. Export cancelled.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        try
        {
            if (formatChoice == "1")
            {
                if (!outputPath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".csv";
                }
                ExportService.ExportContentToCsv(_contentItems, properties, outputPath);
            }
            else if (formatChoice == "2")
            {
                if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".json";
                }
                ExportService.ExportContentToJson(_contentItems, properties, outputPath);
            }
            else
            {
                Console.WriteLine("\nInvalid format choice. Export cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError during export: {ex.Message}");
        }

        WaitForKey();

    }

    private void ExportContentTypesMenu()
    {
        LoadContentTypes();

        if (_contentTypes == null || _contentTypes.Count == 0)
        {
            Console.WriteLine("\nNo content types found in the package.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        Console.WriteLine("\n─────────────────────────────────────────────────────────");
        Console.WriteLine("Export Content Types");
        Console.WriteLine("─────────────────────────────────────────────────────────");

        Console.WriteLine("\nSelect export format:");
        Console.WriteLine("  1. CSV");
        Console.WriteLine("  2. JSON");
        Console.Write("> ");
        var formatChoice = Console.ReadLine()?.Trim();

        Console.Write("\nEnter output file path: ");
        var outputPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine("\nNo output path specified. Export cancelled.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        try
        {
            if (formatChoice == "1")
            {
                if (!outputPath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".csv";
                }
                ExportService.ExportContentTypesToCsv(_contentTypes, outputPath);
            }
            else if (formatChoice == "2")
            {
                if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".json";
                }
                ExportService.ExportContentTypesToJson(_contentTypes, outputPath);
            }
            else
            {
                Console.WriteLine("\nInvalid format choice. Export cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError during export: {ex.Message}");
        }

        WaitForKey();

    }

    private void ExportCategoriesMenu()
    {
        LoadCategories();

        if (_categories == null || _categories.Count == 0)
        {
            Console.WriteLine("\nNo categories found in the package.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        Console.WriteLine("\n─────────────────────────────────────────────────────────");
        Console.WriteLine("Export Categories");
        Console.WriteLine("─────────────────────────────────────────────────────────");

        Console.WriteLine("\nSelect export format:");
        Console.WriteLine("  1. CSV");
        Console.WriteLine("  2. JSON");
        Console.Write("> ");
        var formatChoice = Console.ReadLine()?.Trim();

        Console.Write("\nEnter output file path: ");
        var outputPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine("\nNo output path specified. Export cancelled.");
            Console.WriteLine("Press any key to continue...");

            return;
        }

        try
        {
            if (formatChoice == "1")
            {
                if (!outputPath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".csv";
                }
                ExportService.ExportCategoriesToCsv(_categories, outputPath);
            }
            else if (formatChoice == "2")
            {
                if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".json";
                }
                ExportService.ExportCategoriesToJson(_categories, outputPath);
            }
            else
            {
                Console.WriteLine("\nInvalid format choice. Export cancelled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError during export: {ex.Message}");
        }

        WaitForKey();

    }

    private void LoadContentItems()
    {
        if (_contentItems == null)
        {
            Console.WriteLine("Loading content items...");
            _contentItems = _packageReader.GetContentItems();
        }
    }

    private void LoadContentTypes()
    {
        if (_contentTypes == null)
        {
            Console.WriteLine("Loading content types...");
            _contentTypes = _packageReader.GetContentTypes();
        }
    }

    private void LoadCategories()
    {
        if (_categories == null)
        {
            Console.WriteLine("Loading categories...");
            _categories = _packageReader.GetCategories();
        }
    }

    private void LoadAudiences()
    {
        if (_audiences == null)
        {
            Console.WriteLine("Loading audiences...");
            _audiences = _packageReader.GetAudiences();
        }
    }
}
