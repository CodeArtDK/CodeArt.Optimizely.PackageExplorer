using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public class ZipPackage : IDisposable
{
    private ZipArchive _zipArchive;

    private Stream zipStream;

    private static readonly Regex NumericCharRefRegex = new("&#(x?[0-9A-Fa-f]+);", RegexOptions.Compiled);

    public ZipPackage(string path)
    {
        var stream = File.OpenRead(path);
        _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
    }

    public ZipPackage(Stream stream)
    {
        zipStream = stream;
        _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
    }

    public XDocument ReadXmlFile(string filename)
    {
        var entry = _zipArchive.Entries.FirstOrDefault(e => e.FullName.Equals(filename, StringComparison.OrdinalIgnoreCase));
        if (entry == null) throw new FileNotFoundException($"File {filename} not found in package.");

        using var stream = entry.Open();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: false);
        var xmlText = reader.ReadToEnd();

        var sanitized = SanitizeXml(xmlText);

        // Use XmlReader with CheckCharacters=false as additional fallback
        using var stringReader = new StringReader(sanitized);
        var settings = new System.Xml.XmlReaderSettings { CheckCharacters = false, DtdProcessing = System.Xml.DtdProcessing.Ignore }; // Ignore DTD to be safe
        using var xmlReader = System.Xml.XmlReader.Create(stringReader, settings);
        return XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace);
    }

    private static string SanitizeXml(string xml)
    {
        if (string.IsNullOrEmpty(xml)) return xml;

        // First remove invalid numeric references like &#2; or &#x2;
        xml = NumericCharRefRegex.Replace(xml, match =>
        {
            var raw = match.Groups[1].Value;
            int codePoint;
            if (raw.StartsWith("x", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(raw.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out codePoint))
                    return match.Value; // keep if parse fails
            }
            else
            {
                if (!int.TryParse(raw, out codePoint)) return match.Value; // keep if parse fails
            }
            return IsValidXmlCodePoint(codePoint) ? match.Value : string.Empty; // remove invalid code point
        });

        // Then strip any remaining invalid literal characters
        var sb = new StringBuilder(xml.Length);
        foreach (var ch in xml)
        {
            if (IsValidXmlChar(ch))
                sb.Append(ch);
        }
        return sb.ToString();
    }

    private static bool IsValidXmlChar(char ch) => IsValidXmlCodePoint(ch);

    private static bool IsValidXmlCodePoint(int codePoint)
    {
        return codePoint == 0x9 || codePoint == 0xA || codePoint == 0xD ||
               (codePoint >= 0x20 && codePoint <= 0xD7FF) ||
               (codePoint >= 0xE000 && codePoint <= 0xFFFD) ||
               (codePoint >= 0x10000 && codePoint <= 0x10FFFF);
    }

    public byte[]? LoadBlobBytes(string blobReference)
    {
        var file = _zipArchive.Entries.FirstOrDefault(f => f.FullName.Replace("/","")==blobReference.Replace("/",""));
        if (file != null)
        {
            using var stream = file.Open();
            BinaryReader br = new BinaryReader(stream);
            return br.ReadBytes((int) file.Length);
        }
        return null;
    }

    public List<string> GetZipEntries()
    {
        return _zipArchive.Entries.Select(e => e.FullName).ToList();
    }

    public void Dispose()
    {
        _zipArchive.Dispose();
    }
}
