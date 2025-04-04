namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class ContentTypeDefinition
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public List<ContentPropertyDefinition> Properties { get; set; } = new();
}

public class ContentPropertyDefinition
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int? TabId { get; set; }
    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsLocalizable { get; set; }
}
