namespace Localize.Data.MySql;

/// <summary>
/// Represents the localize database context.
/// </summary>
public class LocalizeDbContext(DbContextOptions<LocalizeDbContext> options) : LocalizeDbContextBase(options)
{
    /// <inheritdoc/>
    protected override bool SupportsSchemas => false;
}
