namespace CodeArt.Optimizely.PackageExplorer.Core.Models;


public class ContentItem
{
    private string? TryGetProperty(string key)
    {
        if (Properties == null) return null;
        return Properties.FirstOrDefault(p => p.Name == key)?.Value;

    }
    public string? Name
    {
        get
        {
            return TryGetProperty("PageName");
        }
    }

    public string? ContentTypeId
    {
        get
        {
            return TryGetProperty("PageTypeID");
        }
    }

    public string? ContentTypeName
    {
        get
        {
            return TryGetProperty("PageTypeName");
        }
    }

    public List<ContentProperty> Properties { get; set; } = new();


    public override string ToString()
    {
        return $"[{ContentTypeName}] {Name}";
    }
}

public class ContentProperty
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";

    public string? TypeName { get; set; }
    public string? Value { get; set; }

    public int OwnerTab { get; set; }

    public int PropertyDefinitionID { get; set; }
    public override string ToString()
    {
        return Name + ": " + Value?.ToString();
    }
}
