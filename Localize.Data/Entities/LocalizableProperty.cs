namespace Localize.Data.Entities;

/// <summary>
/// A translated field on some other entity. The invariant text stays on that entity.
/// Callers pass <c>typeof(T).FullName</c>, the entity id as text, and the property name.
/// </summary>
public class LocalizableProperty : BaseEntity<int>
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
    /// Gets or sets the entity type.
    /// </summary>
    public string EntityType { get; set; } = "";

    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public string EntityId { get; set; } = "";

    /// <summary>
    /// Gets or sets the property.
    /// </summary>
    public string Property { get; set; } = "";

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Represents the localizable property map.
/// </summary>
public class LocalizablePropertyMap : IEntityTypeConfiguration<LocalizableProperty>
{
    /// <summary>
    /// Configures the value.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<LocalizableProperty> builder)
    {
        builder.ToTable(Constants.Tables.LocalizableProperties, Constants.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.ScopeId).IsRequired().HasMaxLength(Constants.MaxLengths.ScopeId).IsUnicode(false);
        builder.Property(m => m.CultureCode).IsRequired().HasMaxLength(Constants.MaxLengths.CultureCode).IsUnicode(false);
        builder.Property(m => m.EntityType).IsRequired().HasMaxLength(Constants.MaxLengths.EntityType).IsUnicode(false);
        builder.Property(m => m.EntityId).IsRequired().HasMaxLength(Constants.MaxLengths.EntityId).IsUnicode(false);
        builder.Property(m => m.Property).IsRequired().HasMaxLength(Constants.MaxLengths.Property).IsUnicode(false);
        builder.Property(m => m.Value).IsUnicode(true);
        builder.HasIndex(m => new { m.ScopeId, m.CultureCode, m.EntityType, m.EntityId, m.Property }).IsUnique();
    }
}
