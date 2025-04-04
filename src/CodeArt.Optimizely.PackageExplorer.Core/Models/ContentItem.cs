namespace CodeArt.Optimizely.PackageExplorer.Core.Models;


public class ContentItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Guid ContentTypeGuid { get; set; }
    public List<ContentProperty> Properties { get; set; } = new();
}

public class ContentProperty
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string? Value { get; set; }
}
