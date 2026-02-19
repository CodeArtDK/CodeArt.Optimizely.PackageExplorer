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

    public PackageReader(Stream stream)
    {
        _zip = new ZipPackage(stream);
    }

    public byte[]? LoadBlobBytes(string blobReference)
    {
        return _zip.LoadBlobBytes(blobReference);
    }

    public List<string> GetZipEntries()
    {
        return _zip.GetZipEntries();
    }

    public List<ContentItem> GetContentItems()
    {
        var xml = _zip.ReadXmlFile("epix.xml");
        return ContentItemParser.ParseContentItems(xml);
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

    public List<CategoryDefinition> GetCategories()
    {
        var xml = _zip.ReadXmlFile("epiDefinition.xml");
        return ContentTypeParser.ParseCategories(xml);
    }

    public List<VisitorGroup> GetAudiences()
    {
        try
        {
            var handlerMapXml = _zip.ReadXmlFile("handleddata/handlermap.xml");
            return AudienceParser.ParseAudiences(handlerMapXml, _zip);
        }
        catch
        {
            // Return empty list if handleddata folder or files don't exist
            return new List<VisitorGroup>();
        }
    }

    public List<DisplayTemplate> GetDisplayTemplates()
    {
        var xml = _zip.ReadXmlFile("epiDefinition.xml");
        return ContentTypeParser.ParseDisplayTemplates(xml);
    }
}
