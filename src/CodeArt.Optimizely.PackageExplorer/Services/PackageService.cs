using CodeArt.Optimizely.PackageExplorer.Core.Models;
using CodeArt.Optimizely.PackageExplorer.Core.Services;
using System.IO;

namespace CodeArt.Optimizely.PackageExplorer.Services
{
    public class PackageService : IDisposable
    {
        private PackageReader packageReader;
        private Stream stream;

        public List<ContentItem>? ContentItems { get; private set; }
        public List<TabDefinition>? Tabs { get; private set; }
        public List<HierarchicalContentItem>? Hierarchy { get; private set; }
        public List<ContentTypeDefinition>? ContentTypes { get; private set; }
        public List<CategoryDefinition>? Categories { get; private set; }
        public PackageDebugInfo DebugInfo { get; private set; } = new();
        public bool IsInDebugMode => DebugInfo.HasErrors;

        public byte[]? LoadBlobBytes(string blobReference)
        {
            return packageReader.LoadBlobBytes(blobReference);
        }


        public ContentTypeDefinition? GetContentTypeFromContent(ContentItem item)
        {
            return ContentTypes.FirstOrDefault(c => c.Guid.ToString() == item.ContentTypeId);
        }
        public string GetBlobMimetype(string blobReference)
        {
            return MimeTypes.GetMimeType(blobReference);
        }

        public async Task LoadPackage(Stream stream)
        {
            // Reset state
            this.stream = stream;
            DebugInfo = new PackageDebugInfo();
            ContentItems = null;
            ContentTypes = null;
            Categories = null;
            Tabs = null;
            Hierarchy = null;

            await Task.Yield(); // Let the spinner render
            
            try
            {
                packageReader = new PackageReader(stream);
                
                // Get list of files in package for debug info
                try
                {
                    DebugInfo.ZipEntries = packageReader.GetZipEntries();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Reading ZIP entries",
                        Message = "Failed to read ZIP archive entries",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                await Task.Yield();

                // Try to load content items
                try
                {
                    ContentItems = packageReader.GetContentItems();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Loading Content Items (epix.xml)",
                        Message = ex is FileNotFoundException ? "Required file 'epix.xml' not found in package" : "Failed to parse content items",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                await Task.Yield();

                // Try to load content types
                try
                {
                    ContentTypes = packageReader.GetContentTypes();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Loading Content Types (epiDefinition.xml)",
                        Message = ex is FileNotFoundException ? "Required file 'epiDefinition.xml' not found in package" : "Failed to parse content types",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                await Task.Yield();

                // Try to load categories
                try
                {
                    Categories = packageReader.GetCategories();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Loading Categories (epiDefinition.xml)",
                        Message = "Failed to parse categories",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                await Task.Yield();

                // Try to load tabs
                try
                {
                    Tabs = packageReader.GetTabs();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Loading Tabs (epiDefinition.xml)",
                        Message = "Failed to parse tabs",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }

                // Try to build hierarchy (only if we have content items)
                if (ContentItems != null)
                {
                    try
                    {
                        Hierarchy = ContentItemEnricher.EnrichContentItems(ContentItems);
                    }
                    catch (Exception ex)
                    {
                        DebugInfo.Errors.Add(new PackageError
                        {
                            Stage = "Building Content Hierarchy",
                            Message = "Failed to build content hierarchy",
                            Details = ex.Message,
                            StackTrace = ex.StackTrace
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Catch-all for any unexpected errors
                DebugInfo.Errors.Add(new PackageError
                {
                    Stage = "Loading Package",
                    Message = "Unexpected error while loading package",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        public void Dispose()
        {
            // Dispose of the package reader and stream
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
