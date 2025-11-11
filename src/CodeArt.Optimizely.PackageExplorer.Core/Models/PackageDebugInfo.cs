namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class PackageDebugInfo
{
    public List<PackageError> Errors { get; set; } = new();
    public List<string> ZipEntries { get; set; } = new();
    public bool HasErrors => Errors.Count > 0;
}

public class PackageError
{
    public string Stage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}
