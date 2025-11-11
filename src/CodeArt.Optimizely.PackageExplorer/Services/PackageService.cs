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
        
        // Track deleted items
        public HashSet<string> DeletedContentIds { get; private set; } = new();
        public HashSet<string> DeletedContentTypeGuids { get; private set; } = new();
        public HashSet<int> DeletedCategoryIds { get; private set; } = new();
        
        // Track modified properties: Key = "ContentLink|PropertyName", Value = new value
        public Dictionary<string, string> ModifiedContentProperties { get; private set; } = new();
        
        // Track modified content type properties: Key = "TypeGuid|PropertyName", Value = new value
        public Dictionary<string, string> ModifiedContentTypeProperties { get; private set; } = new();
        
        public bool HasModifications => DeletedContentIds.Count > 0 || 
                                       DeletedContentTypeGuids.Count > 0 || 
                                       DeletedCategoryIds.Count > 0 ||
                                       ModifiedContentProperties.Count > 0 ||
                                       ModifiedContentTypeProperties.Count > 0;
        
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
            
            // Reset deletion tracking when loading a new package
            DeletedContentIds.Clear();
            DeletedContentTypeGuids.Clear();
            DeletedCategoryIds.Clear();
            
            // Reset modification tracking
            ModifiedContentProperties.Clear();
            ModifiedContentTypeProperties.Clear();

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
        
        // Methods for modifying properties
        public void UpdateContentProperty(ContentItem item, string propertyName, string newValue)
        {
            if (item.ContentLink == null) return;
            
            var key = $"{item.ContentLink}|{propertyName}";
            var property = item.Properties.FirstOrDefault(p => p.Name == propertyName);
            
            if (property != null)
            {
                // Update the in-memory property value
                property.Value = newValue;
                
                // Track the modification
                ModifiedContentProperties[key] = newValue;
                OnModificationsChanged?.Invoke();
            }
        }
        
        public void UpdateContentTypeProperty(ContentTypeDefinition contentType, ContentPropertyDefinition property, string fieldName, object newValue)
        {
            var key = $"{contentType.Guid}|{property.Name}|{fieldName}";
            
            // Update the in-memory value based on field
            switch (fieldName)
            {
                case "EditCaption":
                    property.EditCaption = newValue?.ToString();
                    break;
                case "IsRequired":
                    property.IsRequired = Convert.ToBoolean(newValue);
                    break;
                case "IsSearchable":
                    property.IsSearchable = Convert.ToBoolean(newValue);
                    break;
                case "IsLocalizable":
                    property.IsLocalizable = Convert.ToBoolean(newValue);
                    break;
            }
            
            // Track the modification
            ModifiedContentTypeProperties[key] = newValue?.ToString() ?? "";
            OnModificationsChanged?.Invoke();
        }
        
        public string? GetModifiedValue(ContentItem item, string propertyName)
        {
            if (item.ContentLink == null) return null;
            var key = $"{item.ContentLink}|{propertyName}";
            return ModifiedContentProperties.TryGetValue(key, out var value) ? value : null;
        }
        
        public bool IsPropertyModified(ContentItem item, string propertyName)
        {
            if (item.ContentLink == null) return false;
            var key = $"{item.ContentLink}|{propertyName}";
            return ModifiedContentProperties.ContainsKey(key);
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
                ModifiedContentProperties,
                ModifiedContentTypeProperties
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
