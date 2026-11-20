using System.IO.Compression;
using System.Text.Json;

namespace Ingest;

public record Snapshot(
    string Retailer,
    string ProductKey,
    string Url,
    DateTime FetchedAt,
    byte[] CompressedData)
{
    public JsonDocument ParseData()
    {
        using var inputStream = new MemoryStream(CompressedData);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);

        return JsonDocument.Parse(gzipStream);
    }
}
