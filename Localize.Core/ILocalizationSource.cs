namespace Localize;

/// <summary>
/// One place strings are read from: a language pack, a RESX set, a JSON folder, or the database.
/// Sources are merged by <see cref="Priority"/>. They do not fall back across cultures; the catalog does that.
/// </summary>
public interface ILocalizationSource
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>Higher wins for the same key in the same culture.</summary>
    int Priority { get; }

    /// <param name="scopeId">Already normalized. File sources ignore it. The database source filters on it.</param>
    /// <param name="cultureCode">Already normalized. Empty is the invariant culture.</param>
    IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode);
}
