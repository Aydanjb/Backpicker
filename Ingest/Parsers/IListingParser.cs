using System.Text.Json;

namespace Ingest.Parsers;

public interface IListingParser
{
    string Retailer { get; }
    ParsedListing? Parse(JsonElement root);
}
