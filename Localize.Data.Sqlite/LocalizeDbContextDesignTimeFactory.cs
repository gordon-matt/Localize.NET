using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.Sqlite;

/// <summary>Used by <c>dotnet ef</c> so migrations can be generated without a host application.</summary>
public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    /// <summary>
    /// Creates the database context.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The database context.</returns>
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Data Source=localize_design.db"));
}
