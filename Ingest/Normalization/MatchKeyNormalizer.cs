using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ingest.Normalization;

public static partial class MatchKeyNormalizer
{
    // Manual normalization, REI and Backcountry sometimes use different names for the same brand
    private static readonly Dictionary<string, string> BrandAliases = new()
    {
        ["nemo equipment"] = "nemo",
        ["osprey packs"] = "osprey",
        ["ignik outdoors"] = "ignik",
    };

    public static string Create(string brand, string name) => $"{NormalizeBrand(brand)}|{Normalize(name)}";

    private static string Normalize(string text)
    {
        var withoutMarks = text.Replace("™", "").Replace("®", "").Replace("©", "").Replace("℠", "");

        var builder = new StringBuilder();
        foreach (var c in withoutMarks.Normalize(NormalizationForm.FormKD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            builder.Append(c switch
            {
                '+' => " plus ",
                _ when char.IsLetterOrDigit(c) => char.ToLowerInvariant(c).ToString(),
                _ => " ",
            });
        }

        return Whitespace().Replace(builder.ToString(), " ").Trim();
    }

    private static string NormalizeBrand(string brand)
    {
        var normalized = LegalSuffix().Replace(Normalize(brand), "");
        return BrandAliases.GetValueOrDefault(normalized, normalized);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    [GeneratedRegex(@"\s+(?:inc|llc|ltd|co|corp)$")]
    private static partial Regex LegalSuffix();
}
