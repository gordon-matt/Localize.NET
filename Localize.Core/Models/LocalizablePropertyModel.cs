namespace Localize.Models;

/// <summary>
/// Represents the localizable property model.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="CultureCode">The culture code.</param>
/// <param name="EntityType">The entity type.</param>
/// <param name="EntityId">The entity identifier.</param>
/// <param name="Property">The property.</param>
/// <param name="Value">The value.</param>
public sealed record LocalizablePropertyModel(
    int Id,
    string CultureCode,
    string EntityType,
    string EntityId,
    string Property,
    string? Value);
