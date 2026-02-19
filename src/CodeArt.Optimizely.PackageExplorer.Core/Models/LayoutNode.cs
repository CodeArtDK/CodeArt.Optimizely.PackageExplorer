namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class LayoutNode
{
    public string Type { get; set; } = "";
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? PropertyBinding { get; set; }
    public string? DisplayTemplateKey { get; set; }
    public Dictionary<string, string> DisplaySettings { get; set; } = new();
    public List<LayoutNode> Nodes { get; set; } = new();
}
