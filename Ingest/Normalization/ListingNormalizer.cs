namespace Ingest.Normalization;

public record NormalizedListing(ParsedListing Listing, string MatchKey, IReadOnlyList<WeightVariant> Weights);

public static class ListingNormalizer
{
    public static NormalizedListing Normalize(ParsedListing listing) =>
        new(
            listing,
            MatchKeyNormalizer.Create(listing.Brand, listing.Name),
            WeightNormalizer.Parse(listing.WeightRaw));
}
