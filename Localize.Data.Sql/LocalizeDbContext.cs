namespace Localize.Data.Sql;

/// <summary>
/// Represents the localize database context.
/// </summary>
public class LocalizeDbContext(DbContextOptions<LocalizeDbContext> options) : LocalizeDbContextBase(options);
