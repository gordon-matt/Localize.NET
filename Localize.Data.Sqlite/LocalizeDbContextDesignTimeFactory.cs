using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.Sqlite;

/// <summary>Used by <c>dotnet ef</c> so migrations can be generated without a host application.</summary>
public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Data Source=localize_design.db"));
}
