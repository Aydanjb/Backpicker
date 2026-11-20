using System.Text.Json;

namespace Ingest.Parsers;

public static class JsonPaths
{
    public static JsonElement? Find(JsonElement element, params string[] path)
    {
        foreach (var key in path)
        {
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(key, out element))
                return null;
        }

        return element;
    }

    public static string? GetString(JsonElement root, params string[] path) =>
        Find(root, path) is { ValueKind: JsonValueKind.String } value && !string.IsNullOrWhiteSpace(value.GetString())
            ? value.GetString()
            : null;

    public static decimal? GetDecimal(JsonElement root, params string[] path) =>
        Find(root, path) is { ValueKind: JsonValueKind.Number } value ? value.GetDecimal() : null;

    public static double? GetDouble(JsonElement root, params string[] path) =>
        Find(root, path) is { ValueKind: JsonValueKind.Number } value ? value.GetDouble() : null;

    public static int? GetInt(JsonElement root, params string[] path) =>
        Find(root, path) is { ValueKind: JsonValueKind.Number } value && value.TryGetInt32(out var number)
            ? number
            : null;

    public static JsonElement? First(JsonElement root, params string[] path) =>
        Find(root, path) is { ValueKind: JsonValueKind.Array } array && array.GetArrayLength() > 0
            ? array[0]
            : null;
}
