using Data;
using Ingest;
using Ingest.Normalization;
using Ingest.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

builder.Services.AddBackpickerDb(RequiredConnectionString("Default"));
builder.Services.AddScoped<ListingLoader>();

using var host = builder.Build();

var reader = new SnapshotReader(RequiredConnectionString("Raw"));
var snapshots = reader.ReadLatestSnapshots();

var parsers = new IListingParser[] { new BackcountryParser(), new ReiParser() }
    .ToDictionary(p => p.Retailer);

List<(Snapshot, NormalizedListing)> parsedListings = [];
var skippedCount = 0;
foreach (var snapshot in snapshots)
{
    if (!parsers.TryGetValue(snapshot.Retailer, out var parser))
        continue;

    using var json = snapshot.ParseData();
    var parsed = parser.Parse(json.RootElement);
    if (parsed is null)
    {
        Console.WriteLine($"Skipped {snapshot.Retailer} {snapshot.ProductKey}: missing a required field");
        skippedCount++;
        continue;
    }

    parsedListings.Add((snapshot, ListingNormalizer.Normalize(parsed)));
}

Console.WriteLine($"Parsed {parsedListings.Count}, skipped {skippedCount}");

using var scope = host.Services.CreateScope();
var loader = scope.ServiceProvider.GetRequiredService<ListingLoader>();
var result = loader.Load(parsedListings);

Console.WriteLine($"Loaded {result.Created} new and {result.Updated} updated listings");
return;

string RequiredConnectionString(string name) =>
    builder.Configuration.GetConnectionString(name)
    ?? throw new InvalidOperationException($"ConnectionStrings:{name} is missing from configuration.");
