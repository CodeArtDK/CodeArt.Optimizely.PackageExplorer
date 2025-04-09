using System.Security.Principal;

namespace CodeArt.Optimizely.PackageExplorer.Core.Models;

public class ContentTypeDefinition
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; } = "";

    public string? GroupName { get; set; }

    public string? Base { get; set; }

    public string? ModelTypeString { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Saved { get; set; }


    public List<ContentPropertyDefinition> Properties { get; set; } = new();

    public override string ToString()
    {
        return Name ?? string.Empty;
    }
}

public class ContentPropertyDefinition
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";

    public int? TabId { get; set; }
    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsLocalizable { get; set; }
    public string? DataType { get; internal set; }
    public int FieldOrder { get; internal set; }
    public string? EditCaption { get; internal set; }
    public bool DisplayEditUI { get; internal set; }
    public bool ExistsOnModel { get; internal set; }

    public override string ToString()
    {
        return (Name ?? string.Empty) + " [" + Type + "]";
    }
}
