using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.Npgsql;

/// <summary>
/// Represents the localize database context design time factory.
/// </summary>
public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    /// <summary>
    /// Creates the database context.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The database context.</returns>
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Host=localhost;Database=localize_design"));
}
