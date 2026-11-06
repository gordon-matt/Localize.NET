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

    /// <summary>
    /// Whether the provider can hold the tables in <see cref="Constants.Schema"/>. Providers where a
    /// schema is a database (MySQL) or does not exist (SQLite) return <see langword="false"/>, and
    /// get <see cref="Constants.TablePrefix"/> on every table name instead.
    /// </summary>
    protected virtual bool SupportsSchemas => true;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(LocalizeDbContextBase).Assembly);

        if (SupportsSchemas)
        {
            builder.HasDefaultSchema(Constants.Schema);
            return;
        }

        builder.Model.SetDefaultSchema(null);

        foreach (var entityType in builder.Model.GetEntityTypes().Where(e => e.BaseType is null))
        {
            if (entityType.GetTableName() is not { } tableName)
            {
                continue;
            }

            entityType.SetSchema(null);
            entityType.SetTableName($"{Constants.TablePrefix}{tableName}");
        }
    }
}
