using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.Npgsql;

public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Host=localhost;Database=localize_design"));
}
