namespace Localize.Data.Entities;

public class Language : BaseEntity<int>
{
    public string ScopeId { get; set; } = "";

    public string Name { get; set; } = "";

    public string CultureCode { get; set; } = "";

    public bool IsRightToLeft { get; set; }

    public bool IsEnabled { get; set; }

    public int SortOrder { get; set; }
}

public class LanguageMap : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(Constants.Tables.Languages, Constants.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.ScopeId).IsRequired().HasMaxLength(Constants.MaxLengths.ScopeId).IsUnicode(false);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(Constants.MaxLengths.Name).IsUnicode(true);
        builder.Property(m => m.CultureCode).IsRequired().HasMaxLength(Constants.MaxLengths.CultureCode).IsUnicode(false);
        builder.Property(m => m.IsRightToLeft).IsRequired();
        builder.Property(m => m.IsEnabled).IsRequired();
        builder.Property(m => m.SortOrder).IsRequired();
        builder.HasIndex(m => new { m.ScopeId, m.CultureCode }).IsUnique();
    }
}
