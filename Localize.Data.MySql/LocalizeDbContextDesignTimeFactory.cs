using Microsoft.EntityFrameworkCore.Design;

namespace Localize.Data.MySql;

public class LocalizeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocalizeDbContext>
{
    public LocalizeDbContext CreateDbContext(string[] args) =>
        new(LocalizeDbContextFactory.BuildOptions("Server=localhost;Database=localize_design"));
}
