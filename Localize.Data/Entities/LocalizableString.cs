namespace Localize.Data.Entities;

/// <summary>One translated UI string. An empty <see cref="CultureCode"/> is the invariant value.</summary>
public class LocalizableString : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the scope identifier.
    /// </summary>
    public string ScopeId { get; set; } = "";

    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode { get; set; } = "";

    /// <summary>
    /// Gets or sets the text key.
    /// </summary>
    public string TextKey { get; set; } = "";

    /// <summary>
    /// Gets or sets the text value.
    /// </summary>
    public string? TextValue { get; set; }
}

/// <summary>
/// Represents the localizable string map.
/// </summary>
public class LocalizableStringMap : IEntityTypeConfiguration<LocalizableString>
{
    /// <summary>
    /// Configures the value.
    /// </summary>
    /// <param name="builder">The builder.</param>
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
