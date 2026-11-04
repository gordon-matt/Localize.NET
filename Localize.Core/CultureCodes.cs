namespace Localize;

/// <summary>Culture codes as Localize stores them. The invariant culture is an empty string, never null.</summary>
public static class CultureCodes
{
    public const string Invariant = "";

    /// <summary>Scope used when the host has a single tenant.</summary>
    public const string GlobalScope = "";

    private static readonly Lazy<IReadOnlyList<CultureChoice>> neutralCultures = new(() => CultureInfo
        .GetCultures(CultureTypes.NeutralCultures)
        .Where(culture => culture.Name.Length > 0 && culture.Parent.Name.Length == 0)
        .Select(ToChoice)
        .OrderBy(choice => choice.DisplayName, StringComparer.CurrentCultureIgnoreCase)
        .ToList());

    // Regions are grouped once by language, so the picker does not walk every culture on each render.
    private static readonly Lazy<Dictionary<string, IReadOnlyList<CultureChoice>>> specificCultures = new(() => CultureInfo
        .GetCultures(CultureTypes.SpecificCultures)
        .Where(culture => culture.Name.Length > 0)
        .Select(culture => (Root: RootNeutral(culture.Name), Choice: ToChoice(culture)))
        .Where(entry => entry.Root is not null)
        .GroupBy(entry => entry.Root!, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<CultureChoice>)group
                .Select(entry => entry.Choice)
                .OrderBy(choice => choice.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToList(),
            StringComparer.OrdinalIgnoreCase));

    private static CultureChoice ToChoice(CultureInfo culture) => new(culture.Name, culture.DisplayName);

    public static string Normalize(string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return Invariant;
        }

        string trimmed = cultureCode.Trim();
        if (trimmed.Equals("invariant", StringComparison.OrdinalIgnoreCase))
        {
            return Invariant;
        }

        try
        {
            return CultureInfo.GetCultureInfo(trimmed).Name;
        }
        catch (CultureNotFoundException)
        {
            return trimmed;
        }
    }

    public static string NormalizeScope(string? scopeId) =>
        string.IsNullOrWhiteSpace(scopeId) ? GlobalScope : scopeId.Trim();

    public static bool IsInvariant(string? cultureCode) => Normalize(cultureCode).Length == 0;

    public static bool IsKnown(string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return false;
        }

        try
        {
            _ = CultureInfo.GetCultureInfo(cultureCode.Trim());
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    public static string DisplayName(string cultureCode)
    {
        try
        {
            return CultureInfo.GetCultureInfo(cultureCode).DisplayName;
        }
        catch (CultureNotFoundException)
        {
            return cultureCode;
        }
    }

    /// <summary>
    /// The top-level neutral cultures (a language with no region), by display name.
    /// Intermediate ones such as zh-Hans are left out; their regions are listed under the language they belong to.
    /// </summary>
    public static IReadOnlyList<CultureChoice> NeutralCultures() => neutralCultures.Value;

    /// <summary>The regional cultures of a language, by display name. Empty for a code that is not a known language.</summary>
    public static IReadOnlyList<CultureChoice> SpecificCulturesOf(string? neutralCode)
    {
        string? root = RootNeutral(neutralCode);
        return root is not null && specificCultures.Value.TryGetValue(root, out var choices) ? choices : [];
    }

    /// <summary>
    /// The language a culture belongs to: <c>fr</c> for <c>fr-CA</c>, and <c>zh</c> for <c>zh-Hans-CN</c>.
    /// Null for a blank or unknown code.
    /// </summary>
    public static string? RootNeutral(string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return null;
        }

        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode.Trim());
            if (culture.Name.Length == 0)
            {
                return null;
            }

            while (culture.Parent.Name.Length > 0)
            {
                culture = culture.Parent;
            }

            return culture.Name;
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    public static bool IsRightToLeft(string cultureCode)
    {
        try
        {
            return CultureInfo.GetCultureInfo(cultureCode).TextInfo.IsRightToLeft;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Requested culture, then each parent when <paramref name="includeParents"/> is true, then invariant.
    /// The list is specific-first. Invariant is always last so a missing translation can fall back.
    /// </summary>
    public static IReadOnlyList<string> FallbackChain(string? cultureCode, bool includeParents)
    {
        var chain = new List<string>();
        string current = Normalize(cultureCode);

        if (current.Length > 0)
        {
            chain.Add(current);

            if (includeParents)
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(current);
                    while (!string.IsNullOrEmpty(culture.Parent.Name))
                    {
                        culture = culture.Parent;
                        chain.Add(culture.Name);
                    }
                }
                catch (CultureNotFoundException)
                {
                    // A code .NET does not know still falls back to the invariant strings.
                }
            }
        }

        chain.Add(Invariant);
        return chain;
    }
}