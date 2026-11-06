namespace Localize.Data.Entities;

/// <summary>
/// Represents the language.
/// </summary>
public class Language : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the scope identifier.
    /// </summary>
    public string ScopeId { get; set; } = "";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether this instance is right to left.
    /// </summary>
    public bool IsRightToLeft { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// Represents the language map.
/// </summary>
public class LanguageMap : IEntityTypeConfiguration<Language>
{
    /// <summary>
    /// Configures the value.
    /// </summary>
    /// <param name="builder">The builder.</param>
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
