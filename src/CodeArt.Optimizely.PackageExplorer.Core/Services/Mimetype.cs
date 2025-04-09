using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services
{
    public static class MimeTypes
    {
        private static readonly Dictionary<string, string> _map = new()
        {
            [".png"] = "image/png",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".gif"] = "image/gif",
            [".svg"] = "image/svg+xml",
            [".mp4"] = "video/mp4",
            [".webm"] = "video/webm",
            [".mp3"] = "audio/mpeg",
            [".pdf"] = "application/pdf",
            [".zip"] = "application/zip"
            // Add more as needed
        };

        public static string GetMimeType(string filename)
        {
            var ext = Path.GetExtension(filename)?.ToLowerInvariant();
            return ext != null && _map.TryGetValue(ext, out var mime) ? mime : "application/octet-stream";
        }
    }
}
