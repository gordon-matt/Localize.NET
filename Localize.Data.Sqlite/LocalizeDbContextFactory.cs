using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Localize.Data.Sqlite;

public class LocalizeDbContextFactory(string connectionString) : ILocalizeDbContextFactory
{
    private DbContextOptions<LocalizeDbContext> Options => field ??= BuildOptions(connectionString);

    public string ProviderName => Constants.DatabaseProviders.Sqlite;

    public DbContext GetContext() => new LocalizeDbContext(Options);

    public DbContext GetContext(string connectionString) => new LocalizeDbContext(BuildOptions(connectionString));

    internal static DbContextOptions<LocalizeDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizeDbContext>();

        optionsBuilder.UseSqlite(Prepare(connectionString), sqlite => sqlite
            .MigrationsAssembly(typeof(LocalizeDbContext).Assembly.GetName().Name)
            .MigrationsHistoryTable(Constants.MigrationsHistoryTable, Constants.Schema));

        // SQLite has no schemas. HasDefaultSchema only names the tables; the warning is noise.
        optionsBuilder.ConfigureWarnings(w => w.Ignore(SqliteEventId.SchemaConfiguredWarning));

        return optionsBuilder.Options;
    }

    /// <summary>
    /// Creates the database's folder. SQLite creates a missing file but not a missing directory,
    /// and turns foreign keys on.
    /// </summary>
    private static string Prepare(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString) { ForeignKeys = true };

        if (builder.DataSource is { Length: > 0 } dataSource
            && !dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
            && builder.Mode != SqliteOpenMode.Memory
            && Path.GetDirectoryName(Path.GetFullPath(dataSource)) is { Length: > 0 } directory)
        {
            Directory.CreateDirectory(directory);
        }

        return builder.ToString();
    }
}
