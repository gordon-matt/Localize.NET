using Localize.Sources;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Localize.Tests;

public class LocalizationCatalogTests
{
    [Fact]
    public void Specific_culture_and_higher_priority_win()
    {
        var sources = new ILocalizationSource[]
        {
            new MemorySource("packs", SourcePriority.LanguagePack, new Dictionary<string, Dictionary<string, string>>
            {
                [""] = new() { ["App/Home.Title"] = "Hello" },
            }),
            new MemorySource("files", SourcePriority.Json, new Dictionary<string, Dictionary<string, string>>
            {
                ["fr-FR"] = new() { ["App/Home.Title"] = "Bonjour" },
            }),
            new MemorySource("database", SourcePriority.Database, new Dictionary<string, Dictionary<string, string>>
            {
                ["fr-FR"] = new() { ["App/Home.Title"] = "Salut" },
            }),
        };

        var catalog = NewCatalog(sources, parentFallback: true);

        Assert.Equal("Salut", catalog.GetString(null, "fr-FR", "App/Home.Title"));
        Assert.Equal("Hello", catalog.GetString(null, "de-DE", "App/Home.Title"));
        Assert.Equal("Hello", catalog.GetString(null, "en-AU", "App/Home.Title"));

        var frenchOnly = catalog.GetCulture(null, "fr-FR");
        Assert.Equal("Salut", frenchOnly["App/Home.Title"]);
        Assert.False(catalog.GetCulture(null, "de-DE").ContainsKey("App/Home.Title"));
    }

    [Fact]
    public void Parent_culture_fills_a_gap()
    {
        var sources = new ILocalizationSource[]
        {
            new MemorySource("files", SourcePriority.Json, new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new() { ["App/Home.Title"] = "Hi" },
            }),
        };

        var catalog = NewCatalog(sources, parentFallback: true);

        Assert.Equal("Hi", catalog.GetString(null, "en-AU", "App/Home.Title"));
    }

    private static LocalizationCatalog NewCatalog(IEnumerable<ILocalizationSource> sources, bool parentFallback) =>
        new(
            new MemoryCache(new MemoryCacheOptions()),
            new LocalizationCache(),
            Options.Create(new LocalizeOptions { UseParentCultureFallback = parentFallback }),
            sources);

    private sealed class MemorySource(
        string name,
        int priority,
        Dictionary<string, Dictionary<string, string>> values) : ILocalizationSource
    {
        public string Name => name;

        public int Priority => priority;

        public IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode) =>
            values.TryGetValue(cultureCode, out var entries)
                ? entries
                : new Dictionary<string, string>();
    }
}
