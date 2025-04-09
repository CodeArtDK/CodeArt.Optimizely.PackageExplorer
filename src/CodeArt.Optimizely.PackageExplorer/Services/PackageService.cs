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
