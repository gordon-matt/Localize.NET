namespace Localize.Sources;

/// <summary>Editor overrides. A null text value is skipped so the catalog can fall back.</summary>
public sealed class DatabaseLocalizationSource(IRepository<LocalizableString> strings) : ILocalizationSource
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => "Database";

    /// <summary>
    /// Gets the priority.
    /// </summary>
    public int Priority => SourcePriority.Database;

    /// <summary>
    /// Gets the all.
    /// </summary>
    /// <param name="scopeId">The scope identifier.</param>
    /// <param name="cultureCode">The culture code.</param>
    /// <returns>The all.</returns>
    public IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode)
    {
        var rows = strings.Find(new SearchOptions<LocalizableString>
        {
            Query = row => row.ScopeId == scopeId && row.CultureCode == cultureCode,
        });

        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            if (!string.IsNullOrEmpty(row.TextKey) && row.TextValue is not null)
            {
                map[row.TextKey] = row.TextValue;
            }
        }

        return map;
    }
}
