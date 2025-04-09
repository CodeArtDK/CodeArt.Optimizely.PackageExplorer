using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services;

public class ZipPackage : IDisposable
{
    private ZipArchive _zipArchive;

    private Stream zipStream;

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
        return XDocument.Load(stream);
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

    public void Dispose()
    {
        _zipArchive.Dispose();
    }
}
