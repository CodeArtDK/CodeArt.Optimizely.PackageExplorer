using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Xml.Linq;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public static class XmlParser
{
    public static List<ContentItem> ParseContentItems(XDocument doc)
    {
        // TODO: Parse XML properly, this is just a stub
        return new List<ContentItem>
        {
            new ContentItem { Id = "1", Title = "Start", ContentType = "StartPage" }
        };
    }


}
