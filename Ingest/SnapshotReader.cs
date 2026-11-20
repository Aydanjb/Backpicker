using Microsoft.Data.Sqlite;

namespace Ingest;

public class SnapshotReader(string connectionString)
{
    public List<Snapshot> ReadLatestSnapshots()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            WITH ranked AS (
                SELECT retailer, product_key, url, fetched_at, data,
                       ROW_NUMBER() OVER (
                           PARTITION BY retailer, product_key
                           ORDER BY fetched_at DESC
                       ) AS rn
                FROM snapshots
            )
            SELECT retailer, product_key, url, fetched_at, data
            FROM ranked
            WHERE rn = 1
            """;

        List<Snapshot> snapshots = [];
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            snapshots.Add(new Snapshot(
                Retailer: reader.GetString(0),
                ProductKey: reader.GetString(1),
                Url: reader.GetString(2),
                FetchedAt: DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc),
                CompressedData: reader.GetFieldValue<byte[]>(4)));
        }

        return snapshots;
    }
}
