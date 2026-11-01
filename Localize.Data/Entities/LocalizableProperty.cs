namespace Localize.Data.Entities;

/// <summary>
/// A translated field on some other entity. The invariant text stays on that entity.
/// Callers pass <c>typeof(T).FullName</c>, the entity id as text, and the property name.
/// </summary>
public class LocalizableProperty : BaseEntity<int>
{
    public string ScopeId { get; set; } = "";

    public string CultureCode { get; set; } = "";

    public string EntityType { get; set; } = "";

    public string EntityId { get; set; } = "";

    public string Property { get; set; } = "";

    public string? Value { get; set; }
}

public class LocalizablePropertyMap : IEntityTypeConfiguration<LocalizableProperty>
{
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
