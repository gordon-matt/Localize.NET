namespace Localize;

/// <summary>Culture codes as Localize stores them. The invariant culture is an empty string, never null.</summary>
public static class CultureCodes
{
    public const string Invariant = "";

    /// <summary>Scope used when the host has a single tenant.</summary>
    public const string GlobalScope = "";

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
