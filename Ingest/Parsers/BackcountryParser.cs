using System.Text.Json;
using static Ingest.Parsers.JsonPaths;

namespace Ingest.Parsers;

public class BackcountryParser : IListingParser
{
    private const string BaseUrl = "https://www.backcountry.com";

    public string Retailer => "backcountry";

    public ParsedListing? Parse(JsonElement root)
    {
        var sku = GetString(root, "id");
        var name = GetString(root, "title");
        var brand = GetString(root, "brand", "name");
        var price = GetDecimal(root, "minSalePrice");

        if (sku is null || name is null || brand is null || price is null)
            return null;

        var reviewCount = GetInt(root, "customerReviews", "count") ?? 0;
        var imagePath = GetString(root, "productMainImage", "largeImg");

        return new ParsedListing(
            Sku: sku,
            Name: name,
            Brand: brand,
            PriceCents: (int)(price.Value * 100),
            Rating: reviewCount > 0 ? GetDouble(root, "customerReviews", "average") : null,
            ReviewCount: reviewCount,
            ImageUrl: imagePath is null ? null : BaseUrl + imagePath,
            CategoryRaw: GetString(root, "category", "name"),
            WeightRaw: FindFeature(root, "Claimed Weight")
                ?? FindFeature(root, "Packed Weight")
                ?? FindFeature(root, "Trail Weight")
                ?? FindFeature(root, "Fill Weight"));
    }

    private static string? FindFeature(JsonElement root, string featureName)
    {
        if (Find(root, "features") is not { ValueKind: JsonValueKind.Array } features)
            return null;

        foreach (var feature in features.EnumerateArray())
        {
            if (GetString(feature, "name") == featureName)
                return GetString(feature, "value");
        }

        return null;
    }
}
