using CodeArt.Optimizely.PackageExplorer.CLI.Services;
using CodeArt.Optimizely.PackageExplorer.Core.Services;
using System.CommandLine;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Optimizely Package Explorer - CLI tool for exploring and exporting Optimizely content packages");

        var fileArgument = new Argument<string>(
            name: "package-file",
            description: "Path to the .episerverdata or .episerverpackage file");

        var listCommand = new Command("list", "List items from the package")
        {
            fileArgument
        };

        var typeOption = new Option<string>(
            name: "--type",
            description: "Type of items to list: content, content-types, media, categories, audiences")
        { IsRequired = true };

        listCommand.AddOption(typeOption);

        listCommand.SetHandler((string file, string type) =>
        {
            ExecuteListCommand(file, type);
        }, fileArgument, typeOption);

        var exportCommand = new Command("export", "Export items from the package")
        {
            fileArgument
        };

        var exportTypeOption = new Option<string>(
            name: "--type",
            description: "Type of items to export: content, content-types, categories")
        { IsRequired = true };

        var formatOption = new Option<string>(
            name: "--format",
            description: "Export format: csv or json")
        { IsRequired = true };

        var outputOption = new Option<string>(
            name: "--output",
            description: "Output file path")
        { IsRequired = true };

        var propertiesOption = new Option<string[]>(
            name: "--properties",
            description: "Properties to export (comma-separated, only for content export)")
        { AllowMultipleArgumentsPerToken = true };

        exportCommand.AddOption(exportTypeOption);
        exportCommand.AddOption(formatOption);
        exportCommand.AddOption(outputOption);
        exportCommand.AddOption(propertiesOption);

        exportCommand.SetHandler((string file, string type, string format, string output, string[] properties) =>
        {
            ExecuteExportCommand(file, type, format, output, properties);
        }, fileArgument, exportTypeOption, formatOption, outputOption, propertiesOption);

        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(exportCommand);

        // If no arguments provided, or just a file path, show interactive menu
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: Provide a package file path to start interactive mode, or use commands:");
            Console.WriteLine("  list --type <content|content-types|media|categories|audiences> <package-file>");
            Console.WriteLine("  export --type <content|content-types|categories> --format <csv|json> --output <file> [--properties <prop1,prop2,...>] <package-file>");
            Console.WriteLine("\nExample:");
            Console.WriteLine("  PackageExplorer.CLI.exe package.episerverdata");
            Console.WriteLine("  PackageExplorer.CLI.exe list --type content package.episerverdata");
            Console.WriteLine("  PackageExplorer.CLI.exe export --type content --format csv --output output.csv --properties PageName,PageTypeName package.episerverdata");
            return 0;
        }
        else if (args.Length == 1 && File.Exists(args[0]))
        {
            // Interactive mode
            var menu = new InteractiveMenu(args[0]);
            menu.Run();
            return 0;
        }

        return await rootCommand.InvokeAsync(args);
    }

    private static void ExecuteListCommand(string packageFile, string type)
    {
        if (!File.Exists(packageFile))
        {
            Console.WriteLine($"Error: File '{packageFile}' not found.");
            return;
        }

        try
        {
            var reader = new PackageReader(packageFile);

            switch (type.ToLowerInvariant())
            {
                case "content":
                    var contentItems = reader.GetContentItems();
                    DisplayService.DisplayContentSummary(contentItems);
                    DisplayService.DisplayContentList(contentItems, 100);
                    break;

                case "content-types":
                    var contentTypes = reader.GetContentTypes();
                    DisplayService.DisplayContentTypesSummary(contentTypes);
                    DisplayService.DisplayContentTypesList(contentTypes, 100);
                    break;

                case "media":
                    var allItems = reader.GetContentItems();
                    var mediaItems = allItems
                        .Where(i => i.ContentTypeName?.Contains("Media", StringComparison.OrdinalIgnoreCase) == true ||
                                   i.ContentTypeName?.Contains("Image", StringComparison.OrdinalIgnoreCase) == true ||
                                   i.ContentTypeName?.Contains("Video", StringComparison.OrdinalIgnoreCase) == true ||
                                   i.ContentTypeName?.Contains("File", StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                    DisplayService.DisplayMediaSummary(mediaItems);
                    DisplayService.DisplayMediaList(mediaItems, 100);
                    break;

                case "categories":
                    var categories = reader.GetCategories();
                    DisplayService.DisplayCategoriesSummary(categories);
                    DisplayService.DisplayCategoriesList(categories, 100);
                    break;

                case "audiences":
                    var audiences = reader.GetAudiences();
                    DisplayService.DisplayAudiencesSummary(audiences);
                    DisplayService.DisplayAudiencesList(audiences, 100);
                    break;

                default:
                    Console.WriteLine($"Unknown type: {type}. Valid types are: content, content-types, media, categories, audiences");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ExecuteExportCommand(string packageFile, string type, string format, string output, string[] properties)
    {
        if (!File.Exists(packageFile))
        {
            Console.WriteLine($"Error: File '{packageFile}' not found.");
            return;
        }

        try
        {
            var reader = new PackageReader(packageFile);
            var formatLower = format.ToLowerInvariant();

            switch (type.ToLowerInvariant())
            {
                case "content":
                    if (properties == null || properties.Length == 0)
                    {
                        Console.WriteLine("Error: --properties option is required for content export.");
                        Console.WriteLine("Example: --properties PageName,PageTypeName,PageLink");
                        return;
                    }

                    var propList = properties.SelectMany(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                            .Select(p => p.Trim())
                                            .ToList();

                    // Check file size to determine if we should use streaming
                    Console.WriteLine("Analyzing package size...");
                    var itemCount = reader.CountContentItems();
                    Console.WriteLine($"Package contains {itemCount} content items.");

                    if (itemCount > 10000)
                    {
                        Console.WriteLine("Large package detected. Using optimized streaming export...");
                        if (formatLower == "csv")
                        {
                            ExportService.ExportContentToCsvStreaming(reader, propList, output);
                        }
                        else if (formatLower == "json")
                        {
                            ExportService.ExportContentToJsonStreaming(reader, propList, output);
                        }
                        else
                        {
                            Console.WriteLine($"Unknown format: {format}. Valid formats are: csv, json");
                        }
                    }
                    else
                    {
                        var contentItems = reader.GetContentItems();
                        if (formatLower == "csv")
                        {
                            ExportService.ExportContentToCsv(contentItems, propList, output);
                        }
                        else if (formatLower == "json")
                        {
                            ExportService.ExportContentToJson(contentItems, propList, output);
                        }
                        else
                        {
                            Console.WriteLine($"Unknown format: {format}. Valid formats are: csv, json");
                        }
                    }
                    break;

                case "content-types":
                    var contentTypes = reader.GetContentTypes();
                    if (formatLower == "csv")
                    {
                        ExportService.ExportContentTypesToCsv(contentTypes, output);
                    }
                    else if (formatLower == "json")
                    {
                        ExportService.ExportContentTypesToJson(contentTypes, output);
                    }
                    else
                    {
                        Console.WriteLine($"Unknown format: {format}. Valid formats are: csv, json");
                    }
                    break;

                case "categories":
                    var categories = reader.GetCategories();
                    if (formatLower == "csv")
                    {
                        ExportService.ExportCategoriesToCsv(categories, output);
                    }
                    else if (formatLower == "json")
                    {
                        ExportService.ExportCategoriesToJson(categories, output);
                    }
                    else
                    {
                        Console.WriteLine($"Unknown format: {format}. Valid formats are: csv, json");
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown type: {type}. Valid types are: content, content-types, categories");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}