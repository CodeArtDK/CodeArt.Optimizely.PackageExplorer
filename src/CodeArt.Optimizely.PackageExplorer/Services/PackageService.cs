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

        public async Task LoadPackage(Stream stream)
        {
            // Load the package from the stream
            this.stream = stream;
            packageReader = new PackageReader(stream);
            ContentItems = packageReader.GetContentItems();
            ContentTypes = packageReader.GetContentTypes();
            Categories = packageReader.GetCategories();
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
