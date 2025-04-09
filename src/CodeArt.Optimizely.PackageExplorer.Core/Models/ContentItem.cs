namespace CodeArt.Optimizely.PackageExplorer.Core.Models;


public class ContentItem
{
    public string? TryGetProperty(string key)
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

    public string? ContentLink
    {
        get
        {
            return TryGetProperty("PageLink")?.Replace("[", "").Replace("]", "").Trim();
        }
    }

    public string? ParentLink
    {
        get
        {
            return TryGetProperty("PageParentLink")?.Replace("[", "").Replace("]", "").Trim();
        }
    }

    public string? ContentAssetsID
    {
        get
        {
            return TryGetProperty("PageContentAssetsID");
        }
    }

    public string? PageLanguageBranch
    {
        get
        {
            return TryGetProperty("PageLanguageBranch");
        }
    }

    public string? PageMasterLanguageBranch
    {
        get
        {
            return TryGetProperty("PageMasterLanguageBranch");
        }
    }

    public string? PageURLSegment
    {
        get
        {
            return TryGetProperty("PageURLSegment");
        }
    }

    public string? EPiSystemReference
    {
        get
        {
            return TryGetProperty("EPi:SystemReference");
        }
    }

    public DateTime? PageStartPublish
    {
        get
        {
            var value = TryGetProperty("PageStartPublish");
            if (value == null) return null;
            if (DateTime.TryParse(value, out var date))
            {
                return date;
            }
            return null;
        }
    }

    public IEnumerable<ContentProperty> CustomProperties
    {
        get
        {
            return Properties.Where(p => !p.Name.StartsWith("Page"));
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
