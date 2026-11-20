using System.Text.Json;
using static Ingest.Parsers.JsonPaths;

namespace Ingest.Parsers;

public class ReiParser : IListingParser
{
    private const string BaseUrl = "https://www.rei.com";

    public string Retailer => "rei";

    public ParsedListing? Parse(JsonElement root)
    {
        var sku = GetString(root, "styleId");
        var name = GetString(root, "title");
        var brand = GetString(root, "brand", "name");
        var price = GetDecimal(root, "stylePrice", "minPrice", "value");

        if (sku is null || name is null || brand is null || price is null)
            return null;

        var reviewCount = GetInt(root, "reviewSummary", "count") ?? 0;
        var imagePath = First(root, "images") is { } image ? GetString(image, "uri") : null;

        return new ParsedListing(
            Sku: sku,
            Name: name,
            Brand: brand,
            PriceCents: (int)(price.Value * 100),
            Rating: reviewCount > 0 ? GetDouble(root, "reviewSummary", "averageRating") : null,
            ReviewCount: reviewCount,
            ImageUrl: imagePath is null ? null : BaseUrl + imagePath,
            CategoryRaw: GetString(root, "primaryCategory", "name"),
            WeightRaw: FindTechSpec(root, "Weight")
                ?? FindTechSpec(root, "Minimum Trail Weight")
                ?? FindTechSpec(root, "Packaged Weight"));
    }

    private static string? FindTechSpec(JsonElement root, string specName)
    {
        if (Find(root, "techSpecs") is not { ValueKind: JsonValueKind.Array } specs)
            return null;

        foreach (var spec in specs.EnumerateArray())
        {
            if (GetString(spec, "name") != specName)
                continue;

            if (Find(spec, "values") is not { ValueKind: JsonValueKind.Array } values)
                return null;

            var parts = values.EnumerateArray()
                .Where(v => v.ValueKind == JsonValueKind.String)
                .Select(v => v.GetString())
                .Where(v => !string.IsNullOrWhiteSpace(v));
            var joined = string.Join("; ", parts);

            return joined.Length > 0 ? joined : null;
        }

        return null;
    }
}
