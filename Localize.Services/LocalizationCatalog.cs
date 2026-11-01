using Microsoft.Extensions.Caching.Memory;

namespace Localize;

public sealed class LocalizationCatalog(
    IMemoryCache memory,
    ILocalizationCache cache,
    IOptions<LocalizeOptions> options,
    IEnumerable<ILocalizationSource> sources) : ILocalizationCatalog
{
    private static readonly IReadOnlyDictionary<string, string> Empty = new Dictionary<string, string>();

    public IReadOnlyDictionary<string, string> GetCulture(string? scopeId, string? cultureCode)
    {
        string scope = CultureCodes.NormalizeScope(scopeId);
        string culture = CultureCodes.Normalize(cultureCode);
        string key = $"{cache.Version}|{scope}|exact|{culture}";

        return memory.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, options.Value.CacheMinutes));
            var merged = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var source in sources.OrderBy(item => item.Priority))
            {
                foreach (var (entryKey, value) in source.GetAll(scope, culture))
                {
                    if (!string.IsNullOrEmpty(entryKey))
                    {
                        merged[entryKey] = value;
                    }
                }
            }

            return merged;
        }) ?? Empty;
    }

    public string? GetString(string? scopeId, string? cultureCode, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        var all = GetAll(scopeId, cultureCode, options.Value.UseParentCultureFallback);
        return all.TryGetValue(key, out string? value) ? value : null;
    }

    public IReadOnlyDictionary<string, string> GetAll(string? scopeId, string? cultureCode, bool includeParents)
    {
        string scope = CultureCodes.NormalizeScope(scopeId);
        string culture = CultureCodes.Normalize(cultureCode);
        string key = $"{cache.Version}|{scope}|{culture}|{(includeParents ? 1 : 0)}";

        return memory.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, options.Value.CacheMinutes));
            return Merge(scope, culture, includeParents);
        }) ?? Empty;
    }

    private Dictionary<string, string> Merge(string scope, string culture, bool includeParents)
    {
        var merged = new Dictionary<string, string>(StringComparer.Ordinal);
        var chain = CultureCodes.FallbackChain(culture, includeParents);
        var ordered = sources.OrderBy(source => source.Priority).ToArray();

        for (int index = chain.Count - 1; index >= 0; index--)
        {
            string step = chain[index];
            foreach (var source in ordered)
            {
                foreach (var (entryKey, value) in source.GetAll(scope, step))
                {
                    if (!string.IsNullOrEmpty(entryKey))
                    {
                        merged[entryKey] = value;
                    }
                }
            }
        }

        return merged;
    }
}
