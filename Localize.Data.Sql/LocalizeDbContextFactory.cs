namespace Localize.Data.Sql;

public class LocalizeDbContextFactory(string connectionString) : ILocalizeDbContextFactory
{
    private DbContextOptions<LocalizeDbContext> Options => field ??= BuildOptions(connectionString);

    public string ProviderName => Constants.DatabaseProviders.SqlServer;

    public DbContext GetContext() => new LocalizeDbContext(Options);

    public DbContext GetContext(string connectionString) => new LocalizeDbContext(BuildOptions(connectionString));

    internal static DbContextOptions<LocalizeDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizeDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sql => sql
            .MigrationsAssembly(typeof(LocalizeDbContext).Assembly.GetName().Name)
            .MigrationsHistoryTable(Constants.MigrationsHistoryTable, Constants.Schema));

        return optionsBuilder.Options;
    }
}
