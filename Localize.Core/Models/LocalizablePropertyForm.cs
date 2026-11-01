namespace Localize.Models;

/// <summary>
/// A translated field on an entity. The invariant value stays on the entity.
/// <see cref="EntityType"/> is usually <c>typeof(T).FullName</c> and <see cref="EntityId"/> is the key as text.
/// </summary>
public sealed class LocalizablePropertyForm
{
    public string? CultureCode { get; set; }

    public string EntityType { get; set; } = "";

    public string EntityId { get; set; } = "";

    public string Property { get; set; } = "";

    public string? Value { get; set; }
}
