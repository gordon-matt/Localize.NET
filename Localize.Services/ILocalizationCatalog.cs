namespace Localize;

/// <summary>Merged view of every <see cref="ILocalizationSource"/>, with culture fallback.</summary>
public interface ILocalizationCatalog
{
    IReadOnlyDictionary<string, string> GetAll(string? scopeId, string? cultureCode, bool includeParents);

    /// <summary>Strings written for this culture only. No parent and no invariant fallback.</summary>
    IReadOnlyDictionary<string, string> GetCulture(string? scopeId, string? cultureCode);

    string? GetString(string? scopeId, string? cultureCode, string key);
}
