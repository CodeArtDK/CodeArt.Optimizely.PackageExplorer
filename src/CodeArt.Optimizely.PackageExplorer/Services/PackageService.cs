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
        
        // Track deleted items
        public HashSet<string> DeletedContentIds { get; private set; } = new();
        public HashSet<string> DeletedContentTypeGuids { get; private set; } = new();
        public HashSet<int> DeletedCategoryIds { get; private set; } = new();
        
        public bool HasModifications => DeletedContentIds.Count > 0 || 
                                       DeletedContentTypeGuids.Count > 0 || 
                                       DeletedCategoryIds.Count > 0;

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
            // Load the package from the stream
            this.stream = stream;
            await Task.Yield(); // Let the spinner render
            packageReader = new PackageReader(stream);
            await Task.Yield(); // Let the spinner render
            ContentItems = packageReader.GetContentItems();
            await Task.Yield(); // Let the spinner render
            ContentTypes = packageReader.GetContentTypes();
            await Task.Yield(); // Let the spinner render
            Categories = packageReader.GetCategories();
            await Task.Yield(); // Let the spinner render
            Tabs = packageReader.GetTabs();
            Hierarchy = ContentItemEnricher.EnrichContentItems(ContentItems);
            
            // Reset deletion tracking when loading a new package
            DeletedContentIds.Clear();
            DeletedContentTypeGuids.Clear();
            DeletedCategoryIds.Clear();
        }
        
        public void DeleteContentItem(ContentItem item)
        {
            if (item.ContentLink != null)
            {
                DeletedContentIds.Add(item.ContentLink);
            }
        }
        
        public void DeleteContentType(ContentTypeDefinition contentType)
        {
            DeletedContentTypeGuids.Add(contentType.Guid.ToString());
        }
        
        public void DeleteCategory(CategoryDefinition category)
        {
            DeletedCategoryIds.Add(category.Id);
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
                DeletedCategoryIds
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
