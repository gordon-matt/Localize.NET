namespace Localize.Data;

/// <summary>
/// Represents the localize database context base.
/// </summary>
public abstract class LocalizeDbContextBase(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// Gets the languages.
    /// </summary>
    public DbSet<Language> Languages => Set<Language>();

    /// <summary>
    /// Gets the localizable strings.
    /// </summary>
    public DbSet<LocalizableString> LocalizableStrings => Set<LocalizableString>();

    /// <summary>
    /// Gets the localizable properties.
    /// </summary>
    public DbSet<LocalizableProperty> LocalizableProperties => Set<LocalizableProperty>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Constants.Schema);
        builder.ApplyConfigurationsFromAssembly(typeof(LocalizeDbContextBase).Assembly);
    }
}
