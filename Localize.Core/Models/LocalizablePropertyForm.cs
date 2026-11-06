namespace Localize.Models;

/// <summary>
/// A translated field on an entity. The invariant value stays on the entity.
/// <see cref="EntityType"/> is usually <c>typeof(T).FullName</c> and <see cref="EntityId"/> is the key as text.
/// </summary>
public sealed class LocalizablePropertyForm
{
    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string? CultureCode { get; set; }

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
