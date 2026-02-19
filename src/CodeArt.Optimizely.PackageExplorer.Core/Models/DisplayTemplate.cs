namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class DisplayTemplate
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ContentTypeName { get; set; }
    public string? BaseType { get; set; }
    public string? NodeType { get; set; }
    public bool IsDefault { get; set; }
    public List<DisplayTemplateSetting> Settings { get; set; } = new();

    public string AppliesTo =>
        ContentTypeName ?? BaseType ?? NodeType ?? "";

    public string TargetType =>
        ContentTypeName != null ? "ContentType" :
        BaseType != null ? "BaseType" :
        NodeType != null ? "NodeType" : "Unknown";
}

public class DisplayTemplateSetting
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Editor { get; set; }
    public int SortOrder { get; set; }
    public List<DisplayTemplateChoice> Choices { get; set; } = new();
}

public class DisplayTemplateChoice
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
}
