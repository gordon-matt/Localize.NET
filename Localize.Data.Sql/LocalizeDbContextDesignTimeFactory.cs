using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.Sql;

public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Server=(localdb)\\mssqllocaldb;Database=localize_design;Trusted_Connection=True;TrustServerCertificate=True"));
}
