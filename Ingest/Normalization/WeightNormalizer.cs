using System.Globalization;
using System.Text.RegularExpressions;

namespace Ingest.Normalization;

public record WeightVariant(string Size, string Raw, int Grams);

public static partial class WeightNormalizer
{
    private const double GramsPerPound = 453.592;
    private const double GramsPerOunce = 28.3495;

    public static IReadOnlyList<WeightVariant> Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return [];

        List<WeightVariant> variants = [];
        foreach (var (size, value) in SplitSizes(raw))
        {
            if (ParseGrams(value) is not { } grams || variants.Any(v => v.Size == size))
                continue;

            variants.Add(new WeightVariant(size, value.Trim(' ', ',', ';'), grams));
        }

        return variants;
    }

    private static IEnumerable<(string Size, string Value)> SplitSizes(string raw)
    {
        if (raw.Contains('['))
        {
            var parts = BracketLabel().Split(raw);
            if (!string.IsNullOrWhiteSpace(parts[0].Trim(' ', ',')))
                yield return ("", parts[0]);

            for (var i = 1; i + 1 < parts.Length; i += 2)
                yield return (FormatSize(parts[i]), parts[i + 1]);

            yield break;
        }

        foreach (var segment in raw.Split(';'))
        {
            var colon = segment.IndexOf(':');
            yield return colon >= 0
                ? (FormatSize(segment[..colon]), segment[(colon + 1)..])
                : ("", segment);
        }
    }

    private static int? ParseGrams(string value)
    {
        var match = Amount().Match(Parenthesized().Replace(value, " "));
        if (!match.Success)
            return null;

        var grams = Number(match.Groups["lb"]) * GramsPerPound
            + Number(match.Groups["oz"]) * GramsPerOunce
            + Number(match.Groups["kg"]) * 1000
            + Number(match.Groups["g"]);

        return grams > 0 ? (int)Math.Round(grams) : null;
    }

    private static double Number(Group group) =>
        group.Success ? double.Parse(group.Value, CultureInfo.InvariantCulture) : 0;

    private static string FormatSize(string label)
    {
        var trimmed = label.Trim();
        return trimmed.Length == 0 ? "" : char.ToUpperInvariant(trimmed[0]) + trimmed[1..];
    }

    [GeneratedRegex(@"\[([^\]]+)\]")]
    private static partial Regex BracketLabel();

    [GeneratedRegex(@"\([^)]*\)")]
    private static partial Regex Parenthesized();

    [GeneratedRegex(
        @"(?<lb>\d+(?:\.\d+)?)\s*(?:lbs?|pounds?)\.?(?:\s*(?<oz>\d+(?:\.\d+)?)\s*(?:oz|ounces?)?\.?)?"
        + @"|(?<oz>\d+(?:\.\d+)?)\s*(?:oz|ounces?)\b\.?"
        + @"|(?<kg>\d+(?:\.\d+)?)\s*kg\b"
        + @"|(?<g>\d+(?:\.\d+)?)\s*g\b",
        RegexOptions.IgnoreCase)]
    private static partial Regex Amount();
}
