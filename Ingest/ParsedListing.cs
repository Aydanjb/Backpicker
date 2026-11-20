namespace Ingest;

public record ParsedListing(
    string Sku,
    string Name,
    string Brand,
    int PriceCents,
    double? Rating,
    int ReviewCount,
    string? ImageUrl,
    string? CategoryRaw,
    string? WeightRaw);
