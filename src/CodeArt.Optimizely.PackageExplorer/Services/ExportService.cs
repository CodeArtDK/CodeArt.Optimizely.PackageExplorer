using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Text;
using System.Text.Json;

namespace CodeArt.Optimizely.PackageExplorer.Services
{
    public class ExportService
    {
        public byte[] ExportToJson(IEnumerable<ContentItem> items, List<string> selectedProperties)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Create filtered objects with only selected properties
            var filteredItems = items.Select(item =>
            {
                var filtered = new Dictionary<string, object?>();
                foreach (var propName in selectedProperties)
                {
                    var prop = item.Properties.FirstOrDefault(p => p.Name == propName);
                    filtered[propName] = prop?.Value;
                }
                return filtered;
            });

            var json = JsonSerializer.Serialize(filteredItems, options);
            return Encoding.UTF8.GetBytes(json);
        }

        public byte[] ExportToCsv(IEnumerable<ContentItem> items, List<string> selectedProperties)
        {
            var csv = new StringBuilder();
            
            // Header row with selected properties
            csv.AppendLine(string.Join(",", selectedProperties.Select(EscapeCsv)));

            foreach (var item in items)
            {
                var values = selectedProperties.Select(propName =>
                {
                    var prop = item.Properties.FirstOrDefault(p => p.Name == propName);
                    return EscapeCsv(prop?.Value);
                });
                csv.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
