namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class TabDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? DisplayName { get; set; }
    public int SortIndex { get; set; }
    public bool IsSystemTab { get; set; }
}
