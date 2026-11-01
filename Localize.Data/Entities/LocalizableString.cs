namespace Localize.Data.Entities;

/// <summary>One translated UI string. An empty <see cref="CultureCode"/> is the invariant value.</summary>
public class LocalizableString : BaseEntity<int>
{
    public string ScopeId { get; set; } = "";

    public string CultureCode { get; set; } = "";

    public string TextKey { get; set; } = "";

    public string? TextValue { get; set; }
}

public class LocalizableStringMap : IEntityTypeConfiguration<LocalizableString>
{
    public void Configure(EntityTypeBuilder<LocalizableString> builder)
    {
        builder.ToTable(Constants.Tables.LocalizableStrings, Constants.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.ScopeId).IsRequired().HasMaxLength(Constants.MaxLengths.ScopeId).IsUnicode(false);
        builder.Property(m => m.CultureCode).IsRequired().HasMaxLength(Constants.MaxLengths.CultureCode).IsUnicode(false);
        builder.Property(m => m.TextKey).IsRequired().HasMaxLength(Constants.MaxLengths.TextKey).IsUnicode(true);
        builder.Property(m => m.TextValue).IsUnicode(true);
        builder.HasIndex(m => new { m.ScopeId, m.CultureCode, m.TextKey }).IsUnique();
    }
}
