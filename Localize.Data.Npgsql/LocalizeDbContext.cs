namespace Localize.Data.Npgsql;

/// <summary>
/// Represents the localize database context.
/// </summary>
public class LocalizeDbContext(DbContextOptions<LocalizeDbContext> options) : LocalizeDbContextBase(options);
