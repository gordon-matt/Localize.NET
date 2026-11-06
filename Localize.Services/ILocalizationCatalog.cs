namespace Localize;

/// <summary>Merged view of every <see cref="ILocalizationSource"/>, with culture fallback.</summary>
public interface ILocalizationCatalog
{
    /// <summary>
    /// Gets the all.
    /// </summary>
    /// <param name="scopeId">The scope identifier.</param>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="includeParents">The include parents.</param>
    /// <returns>The all.</returns>
    IReadOnlyDictionary<string, string> GetAll(string? scopeId, string? cultureCode, bool includeParents);

    /// <summary>Strings written for this culture only. No parent and no invariant fallback.</summary>
    IReadOnlyDictionary<string, string> GetCulture(string? scopeId, string? cultureCode);

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="scopeId">The scope identifier.</param>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="key">The key.</param>
    /// <returns>The string.</returns>
    string? GetString(string? scopeId, string? cultureCode, string key);
}
