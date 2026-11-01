namespace Localize.Data;

public abstract class LocalizeDbContextBase(DbContextOptions options) : DbContext(options)
{
    public DbSet<Language> Languages => Set<Language>();

    public DbSet<LocalizableString> LocalizableStrings => Set<LocalizableString>();

    public DbSet<LocalizableProperty> LocalizableProperties => Set<LocalizableProperty>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Constants.Schema);
        builder.ApplyConfigurationsFromAssembly(typeof(LocalizeDbContextBase).Assembly);
    }
}
