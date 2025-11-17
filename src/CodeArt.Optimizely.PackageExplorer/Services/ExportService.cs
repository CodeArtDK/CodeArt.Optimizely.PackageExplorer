using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Text;
using System.Text.Json;
using System.Globalization;

namespace CodeArt.Optimizely.PackageExplorer.Services
{
    public class ExportService
    {
        // Known property names that represent date/time values
        private static readonly HashSet<string> KnownDateProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "PageStartPublish","PageStopPublish","PageChanged","PageCreated","PageModified","PageSaved","PageDeleted",
            "StartPublish","StopPublish","Changed","Created","Modified","Saved","Deleted"
        };

        public byte[] ExportToJson(IEnumerable<ContentItem> items, List<string> selectedProperties)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            var filteredItems = items.Select(item =>
            {
                var filtered = new Dictionary<string, object?>();
                foreach (var propName in selectedProperties)
                {
                    var prop = item.Properties.FirstOrDefault(p => p.Name == propName);
                    filtered[propName] = FormatPropertyValue(propName, prop?.Value);
                }
                return filtered;
            });

            var json = JsonSerializer.Serialize(filteredItems, options);
            return Encoding.UTF8.GetBytes(json);
        }

        public byte[] ExportToCsv(IEnumerable<ContentItem> items, List<string> selectedProperties)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(",", selectedProperties.Select(EscapeCsv)));

            foreach (var item in items)
            {
                var values = selectedProperties.Select(propName =>
                {
                    var prop = item.Properties.FirstOrDefault(p => p.Name == propName);
                    var formatted = FormatPropertyValue(propName, prop?.Value);
                    return EscapeCsv(formatted);
                });
                csv.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string? FormatPropertyValue(string propName, string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue)) return rawValue;

            if (IsLikelyDateProperty(propName, rawValue) && TryParseDate(rawValue, out var dt))
            {
                // ISO 8601 (round-trip) in UTC
                return dt.ToUniversalTime().ToString("O");
            }
            return rawValue;
        }

        private bool IsLikelyDateProperty(string propName, string value)
        {
            if (KnownDateProperties.Contains(propName)) return true;
            // Simple heuristic: presence of typical date separators or 'T'
            if (value.Length >= 6 && value.Length <= 40 && (value.Contains('-') || value.Contains('/') || value.Contains('T')))
                return true;
            return false;
        }

        private bool TryParseDate(string value, out DateTime dt)
        {
            // Try invariant and current culture; assume/adjust to UTC
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
                return true;
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
                return true;
            return false;
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return '"' + value.Replace("\"", "\"\"") + '"';
            return value;
        }
    }
}
