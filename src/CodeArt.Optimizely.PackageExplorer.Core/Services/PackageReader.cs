using CodeArt.Optimizely.PackageExplorer.Core.Models;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public class PackageReader
{
    private readonly string _packagePath;
    private readonly ZipPackage _zip;

    public PackageReader(string packagePath)
    {
        _packagePath = packagePath;
        _zip = new ZipPackage(packagePath);
    }

    public List<ContentItem> GetContentItems()
    {
        var xml = _zip.ReadXmlFile("epix.xml");
        return XmlParser.ParseContentItems(xml);
    }

    public List<TabDefinition> GetTabs()
    {
        var xml = _zip.ReadXmlFile("epiDefinition.xml");
        return ContentTypeParser.ParseTabs(xml);
    }

    public List<ContentTypeDefinition> GetContentTypes()
    {
        var xml = _zip.ReadXmlFile("epiDefinition.xml");
        return ContentTypeParser.ParseContentTypes(xml);
    }

    // Add methods for media, visitor groups, etc.
}
