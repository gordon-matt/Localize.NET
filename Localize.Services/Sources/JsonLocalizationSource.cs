using Microsoft.Extensions.Hosting;

namespace Localize.Sources;

/// <summary>
/// Reads a folder of JSON files. <c>fr-FR.json</c> and <c>fr-FR/*.json</c> are French.
/// <c>default.json</c> and <c>invariant.json</c> are the invariant culture. The file name wins
/// over a culture written inside the file, so the folder layout stays obvious.
/// </summary>
public sealed class JsonLocalizationSource(
    IOptions<LocalizeOptions> options,
    IServiceProvider services,
    ILogger<JsonLocalizationSource> logger) : ILocalizationSource
{
    private static readonly IReadOnlyDictionary<string, string> Empty = new Dictionary<string, string>();

    private readonly object gate = new();

    private Dictionary<string, Dictionary<string, string>>? cached;

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => "Json";

    /// <summary>
    /// Gets the priority.
    /// </summary>
    public int Priority => options.Value.JsonOverridesDatabase ? SourcePriority.JsonOverride : SourcePriority.Json;

    /// <summary>
    /// Gets the all.
    /// </summary>
    /// <param name="scopeId">The scope identifier.</param>
    /// <param name="cultureCode">The culture code.</param>
    /// <returns>The all.</returns>
    public IReadOnlyDictionary<string, string> GetAll(string scopeId, string cultureCode)
    {
        var all = Load();
        return all.TryGetValue(cultureCode, out var entries) ? entries : Empty;
    }

    /// <summary>
    /// Clears the value.
    /// </summary>
    public void Clear()
    {
        lock (gate)
        {
            cached = null;
        }
    }

    /// <summary>
    /// Resolves the directory.
    /// </summary>
    /// <returns>The directory.</returns>
    public string? ResolveDirectory()
    {
        string? configured = options.Value.JsonDirectory;
        if (string.IsNullOrWhiteSpace(configured))
        {
            return null;
        }

        if (Path.IsPathRooted(configured))
        {
            return configured;
        }

        var environment = services.GetService<IHostEnvironment>();
        string root = environment?.ContentRootPath ?? AppContext.BaseDirectory;
        return Path.Combine(root, configured);
    }

    private Dictionary<string, Dictionary<string, string>> Load()
    {
        lock (gate)
        {
            if (cached is not null)
            {
                return cached;
            }

            var merged = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
            string? directory = ResolveDirectory();
            if (directory is null || !Directory.Exists(directory))
            {
                cached = merged;
                return cached;
            }

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories)
                    .Order(StringComparer.OrdinalIgnoreCase);
            }
            catch (IOException ex)
            {
                logger.LogWarning(ex, "Could not list JSON localization files in {Directory}.", directory);
                cached = merged;
                return cached;
            }

            foreach (string path in files)
            {
                string? culture = JsonLocalizationReader.CultureFromPath(path);
                if (culture is null)
                {
                    continue;
                }

                string json;
                try
                {
                    json = File.ReadAllText(path);
                }
                catch (IOException ex)
                {
                    logger.LogWarning(ex, "Could not read {Path}.", path);
                    continue;
                }

                if (!JsonLocalizationReader.TryRead(json, out _, out var strings, out string? error))
                {
                    logger.LogWarning("Skipped {Path}. {Error}", path, error);
                    continue;
                }

                if (!merged.TryGetValue(culture, out var bucket))
                {
                    bucket = new Dictionary<string, string>(StringComparer.Ordinal);
                    merged[culture] = bucket;
                }

                foreach (var (key, value) in strings)
                {
                    bucket[key] = value;
                }
            }

            cached = merged;
            return cached;
        }
    }
}
