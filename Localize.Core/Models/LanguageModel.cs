namespace Localize.Models;

public sealed record LanguageModel(
    int Id,
    string Name,
    string CultureCode,
    bool IsRightToLeft,
    bool IsEnabled,
    int SortOrder);
