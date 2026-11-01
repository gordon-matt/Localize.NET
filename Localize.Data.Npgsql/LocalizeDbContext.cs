namespace Localize.Data.Npgsql;

public class LocalizeDbContext(DbContextOptions<LocalizeDbContext> options) : LocalizeDbContextBase(options);
