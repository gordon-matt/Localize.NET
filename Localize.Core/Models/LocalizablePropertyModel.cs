namespace Localize.Models;

public sealed record LocalizablePropertyModel(
    int Id,
    string CultureCode,
    string EntityType,
    string EntityId,
    string Property,
    string? Value);
