using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System.Text;
using System.Text.Json;

namespace CodeArt.Optimizely.PackageExplorer.Services
{
    public class ExportService
    {
        public byte[] ExportToJson(IEnumerable<ContentItem> items)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(items, options);
            return Encoding.UTF8.GetBytes(json);
        }

        public byte[] ExportToCsv(IEnumerable<ContentItem> items)
        {
            var csv = new StringBuilder();
            
            // Header row
            csv.AppendLine("ContentLink,Name,ContentType,ParentLink,URLSegment,Language,MasterLanguage,StartPublish");

            foreach (var item in items)
            {
                csv.AppendLine($"{EscapeCsv(item.ContentLink)},{EscapeCsv(item.Name)},{EscapeCsv(item.ContentTypeName)},{EscapeCsv(item.ParentLink)},{EscapeCsv(item.PageURLSegment)},{EscapeCsv(item.PageLanguageBranch)},{EscapeCsv(item.PageMasterLanguageBranch)},{EscapeCsv(item.PageStartPublish?.ToString("o"))}");
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
