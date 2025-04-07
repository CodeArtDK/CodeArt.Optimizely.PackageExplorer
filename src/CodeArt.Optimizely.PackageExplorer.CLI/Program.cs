using CodeArt.Optimizely.PackageExplorer.Core.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");


        PackageReader packageReader = new PackageReader("..\\..\\..\\..\\..\\Samples\\DefaultSiteContent.episerverdata");

        var contentItems = packageReader.GetContentItems();


        var hierarchy = ContentItemEnricher.EnrichContentItems(contentItems);


        var tabs = packageReader.GetTabs();
        var contentTypes = packageReader.GetContentTypes();

    }
}