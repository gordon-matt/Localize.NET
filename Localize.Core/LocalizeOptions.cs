namespace Localize;

/// <summary>Bound from the <c>Localize</c> configuration section. Every value has a default.</summary>
public sealed class LocalizeOptions
{
    public const string SectionName = "Localize";

    /// <summary>Culture used when the visitor has not chosen one. Seeded as an enabled language.</summary>
    public string DefaultCulture { get; set; } = "en-US";

    /// <summary>Cookie the culture switch writes.</summary>
    public string CultureCookieName { get; set; } = "CurrentCulture";

    /// <summary><c>fr-FR</c> falls back to <c>fr</c>, then to the invariant strings.</summary>
    public bool UseParentCultureFallback { get; set; } = true;

    /// <summary>
    /// When a rendered key is missing from every source, store it in the database so an editor can
    /// see it. Leave it off for a site that only reads files.
    /// </summary>
    public bool TrackMissingKeys { get; set; }

    /// <summary>
    /// Folder of JSON files, relative to the content root unless it is a full path.
    /// <c>fr-FR.json</c> or <c>fr-FR/Home.json</c>. Empty means JSON is not read.
    /// </summary>
    public string? JsonDirectory { get; set; }

    public bool WatchJsonFiles { get; set; } = true;

    /// <summary>A deployed JSON file wins over a database row. The default lets an editor override the file.</summary>
    public bool JsonOverridesDatabase { get; set; }

    public int CacheMinutes { get; set; } = 30;

    /// <summary>Apply this library's migrations when the host starts.</summary>
    public bool AutoMigrate { get; set; } = true;

    /// <summary>Insert <see cref="DefaultCulture"/> for the global scope when the language table is empty.</summary>
    public bool SeedDefaultLanguage { get; set; } = true;

    /// <summary>Admin pages require a signed-in user. The culture switch stays public.</summary>
    public bool RequireAuthentication { get; set; } = true;

    /// <summary>
    /// Authorization policy for the admin pages.
    /// Empty allows any signed-in user when <see cref="RequireAuthentication"/> is set.
    /// </summary>
    public string? ManagePolicy { get; set; }

    /// <summary>
    /// Import writes a translation only when the invariant culture already has that key.
    /// Leave this off to let a file introduce new keys.
    /// </summary>
    public bool ImportOnlyKnownKeys { get; set; }
}
