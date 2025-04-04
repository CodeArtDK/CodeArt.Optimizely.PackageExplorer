namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class ContentItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public List<KeyValuePair<string, string>> Properties { get; set; } = new();
}