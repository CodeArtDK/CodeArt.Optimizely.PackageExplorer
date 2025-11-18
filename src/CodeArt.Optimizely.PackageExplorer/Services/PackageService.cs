using CodeArt.Optimizely.PackageExplorer.Core.Models;
using CodeArt.Optimizely.PackageExplorer.Core.Services;
using Microsoft.IO;
using System.IO;

namespace CodeArt.Optimizely.PackageExplorer.Services
{
    public class PackageService : IDisposable
    {
        private PackageReader packageReader;
        private Stream stream;
        private static readonly RecyclableMemoryStreamManager _recyclableManager = new();

        public List<ContentItem>? ContentItems { get; private set; }
        public List<TabDefinition>? Tabs { get; private set; }
        public List<HierarchicalContentItem>? Hierarchy { get; private set; }
        public List<ContentTypeDefinition>? ContentTypes { get; private set; }
        public List<CategoryDefinition>? Categories { get; private set; }
        public List<VisitorGroup>? Audiences { get; private set; }
        public PackageDebugInfo DebugInfo { get; private set; } = new();
        public bool IsInDebugMode => DebugInfo.HasErrors;
        
        // Track deleted items
        public HashSet<string> DeletedContentIds { get; private set; } = new();
        public HashSet<string> DeletedContentTypeGuids { get; private set; } = new();
        public HashSet<int> DeletedCategoryIds { get; private set; } = new();
        public HashSet<int> DeletedTabIds { get; private set; } = new();
        
        public bool HasModifications => DeletedContentIds.Count > 0 || 
                                       DeletedContentTypeGuids.Count > 0 || 
                                       DeletedCategoryIds.Count > 0 ||
                                       DeletedTabIds.Count > 0;
        
        // Event to notify when modifications occur
        public event Action? OnModificationsChanged;

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

        public async Task LoadPackage(Stream inputStream)
        {
            // Use recyclable memory stream to buffer the uploaded package only once (if seek needed)
            // BrowserFile streams are forward-only; ZipArchive requires seekable stream for some operations and re-read
            var buffered = _recyclableManager.GetStream("package-buffer");
            await inputStream.CopyToAsync(buffered);
            buffered.Position = 0;
            stream = buffered; // store for export modifications

            // Reset state
            DebugInfo = new PackageDebugInfo();
            ContentItems = null;
            ContentTypes = null;
            Categories = null;
            Audiences = null;
            Tabs = null;
            Hierarchy = null;
            
            // Reset deletion tracking when loading a new package
            DeletedContentIds.Clear();
            DeletedContentTypeGuids.Clear();
            DeletedCategoryIds.Clear();
            DeletedTabIds.Clear();

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

                await Task.Yield();

                // Try to load audiences (visitor groups)
                try
                {
                    Audiences = packageReader.GetAudiences();
                }
                catch (Exception ex)
                {
                    DebugInfo.Errors.Add(new PackageError
                    {
                        Stage = "Loading Audiences (handleddata)",
                        Message = "Failed to parse audiences",
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
        
        public void DeleteContentItem(ContentItem item)
        {
            if (item.ContentLink != null)
            {
                DeletedContentIds.Add(item.ContentLink);
                OnModificationsChanged?.Invoke();
            }
        }
        
        public void DeleteContentType(ContentTypeDefinition contentType)
        {
            DeletedContentTypeGuids.Add(contentType.Guid.ToString());
            OnModificationsChanged?.Invoke();
        }
        
        public void DeleteCategory(CategoryDefinition category)
        {
            DeletedCategoryIds.Add(category.Id);
            OnModificationsChanged?.Invoke();
        }
        
        public void DeleteTab(TabDefinition tab)
        {
            DeletedTabIds.Add(tab.Id);
            OnModificationsChanged?.Invoke();
        }
        
        public bool IsDeleted(ContentItem item)
        {
            return item.ContentLink != null && DeletedContentIds.Contains(item.ContentLink);
        }
        
        public bool IsDeleted(ContentTypeDefinition contentType)
        {
            return DeletedContentTypeGuids.Contains(contentType.Guid.ToString());
        }
        
        public bool IsDeleted(CategoryDefinition category)
        {
            return DeletedCategoryIds.Contains(category.Id);
        }
        
        public bool IsDeleted(TabDefinition tab)
        {
            return DeletedTabIds.Contains(tab.Id);
        }
        
        public Stream ExportModifiedPackage()
        {
            if (stream == null)
            {
                throw new InvalidOperationException("No package is currently loaded");
            }
            
            return PackageWriter.CreateModifiedPackage(
                stream,
                DeletedContentIds,
                DeletedContentTypeGuids,
                DeletedCategoryIds,
                DeletedTabIds
            );
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
