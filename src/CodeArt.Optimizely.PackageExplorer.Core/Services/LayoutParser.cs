using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Text.Json;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public static class LayoutParser
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static LayoutNode? ParseLayout(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var element = JsonSerializer.Deserialize<JsonElement>(json, _options);
            return ParseNode(element);
        }
        catch
        {
            return null;
        }
    }

    private static LayoutNode? ParseNode(JsonElement element, int depth = 0)
    {
        if (depth >= 10 || element.ValueKind != JsonValueKind.Object)
            return null;

        var node = new LayoutNode
        {
            Type = element.TryGetProperty("type", out var t) ? t.GetString() ?? "" : "",
            Name = element.TryGetProperty("name", out var n) ? n.GetString() : null,
            Id = element.TryGetProperty("id", out var id) ? id.GetString() : null,
            PropertyBinding = element.TryGetProperty("propertyBinding", out var pb) ? pb.GetString() : null,
            DisplayTemplateKey = element.TryGetProperty("displayTemplateKey", out var dtk) ? dtk.GetString() : null
        };

        if (element.TryGetProperty("displaySettings", out var ds) && ds.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in ds.EnumerateObject())
            {
                node.DisplaySettings[prop.Name] = prop.Value.ToString();
            }
        }

        if (element.TryGetProperty("nodes", out var nodes) && nodes.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in nodes.EnumerateArray())
            {
                var childNode = ParseNode(child, depth + 1);
                if (childNode != null)
                    node.Nodes.Add(childNode);
            }
        }

        return node;
    }
}
