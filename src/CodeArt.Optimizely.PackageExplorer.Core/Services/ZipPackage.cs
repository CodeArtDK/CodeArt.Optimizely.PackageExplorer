using System.IO.Compression;
using System.Xml.Linq;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public class ZipPackage : IDisposable
{
    private readonly ZipArchive _zipArchive;

    public ZipPackage(string path)
    {
        var stream = File.OpenRead(path);
        _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
    }

    public ZipPackage(Stream stream)
    {
        _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
    }

    public XDocument ReadXmlFile(string filename)
    {
        var entry = _zipArchive.Entries.FirstOrDefault(e => e.FullName.Equals(filename, StringComparison.OrdinalIgnoreCase));
        if (entry == null) throw new FileNotFoundException($"File {filename} not found in package.");

        using var stream = entry.Open();
        return XDocument.Load(stream);
    }

    public void Dispose()
    {
        _zipArchive.Dispose();
    }
}
