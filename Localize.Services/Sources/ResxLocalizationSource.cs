using System.Resources;

namespace Localize.Sources;

/// <summary>Reads RESX resources without walking parent cultures. The catalog does the fallback.</summary>
public sealed class ResxLocalizationSource(
    IEnumerable<ResxResource> resources,
    ILogger<ResxLocalizationSource> logger) : ILocalizationSource
{
    public string Name => "Resx";

    public int Priority => SourcePriority.Resx;

    public IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode)
    {
        var merged = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!resources.Any())
        {
            return merged;
        }

        CultureInfo culture;
        try
        {
            culture = cultureCode.Length == 0
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(cultureCode);
        }
        catch (CultureNotFoundException)
        {
            return merged;
        }

        foreach (var resource in resources)
        {
            try
            {
                var set = resource.Manager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
                if (set is null)
                {
                    continue;
                }

                foreach (System.Collections.DictionaryEntry entry in set)
                {
                    if (entry.Key is string key && entry.Value is string value)
                    {
                        merged[key] = value;
                    }
                }
            }
            catch (Exception ex) when (ex is MissingManifestResourceException or MissingSatelliteAssemblyException)
            {
                logger.LogDebug(ex, "No RESX set for {Culture} in {BaseName}.", culture.Name, resource.Manager.BaseName);
            }
        }

        return merged;
    }
}
