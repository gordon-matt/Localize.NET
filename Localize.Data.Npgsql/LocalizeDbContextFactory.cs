namespace Localize.Data.Npgsql;

public class LocalizeDbContextFactory(string connectionString) : ILocalizeDbContextFactory
{
    private DbContextOptions<LocalizeDbContext> Options => field ??= BuildOptions(connectionString);

    public string ProviderName => Constants.DatabaseProviders.Npgsql;

    public DbContext GetContext() => new LocalizeDbContext(Options);

    public DbContext GetContext(string connectionString) => new LocalizeDbContext(BuildOptions(connectionString));

    internal static DbContextOptions<LocalizeDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizeDbContext>();

        optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql
            .MigrationsAssembly(typeof(LocalizeDbContext).Assembly.GetName().Name)
            .MigrationsHistoryTable(Constants.MigrationsHistoryTable, Constants.Schema));

        return optionsBuilder.Options;
    }
}
