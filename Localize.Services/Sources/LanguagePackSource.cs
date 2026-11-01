namespace Localize.Sources;

/// <summary>Reads <see cref="ILanguagePack"/> registrations. A new scope is created per load so transient packs work.</summary>
public sealed class LanguagePackSource(IServiceScopeFactory scopes) : ILocalizationSource
{
    public string Name => "LanguagePack";

    public int Priority => SourcePriority.LanguagePack;

    public IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode)
    {
        var merged = new Dictionary<string, string>(StringComparer.Ordinal);

        using var scope = scopes.CreateScope();
        foreach (var pack in scope.ServiceProvider.GetServices<ILanguagePack>())
        {
            if (pack.LocalizedStrings is null)
            {
                continue;
            }

            if (!string.Equals(CultureCodes.Normalize(pack.CultureCode), cultureCode, StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var (key, value) in pack.LocalizedStrings)
            {
                if (!string.IsNullOrEmpty(key) && value is not null)
                {
                    merged[key] = value;
                }
            }
        }

        return merged;
    }
}
