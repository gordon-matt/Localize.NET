namespace Localize.Models;

/// <summary>
/// Represents the language model.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
/// <param name="CultureCode">The culture code.</param>
/// <param name="IsRightToLeft">The is right to left.</param>
/// <param name="IsEnabled">The is enabled.</param>
/// <param name="SortOrder">The sort order.</param>
public sealed record LanguageModel(
    int Id,
    string Name,
    string CultureCode,
    bool IsRightToLeft,
    bool IsEnabled,
    int SortOrder);
