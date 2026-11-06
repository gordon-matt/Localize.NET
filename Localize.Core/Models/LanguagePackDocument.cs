namespace Localize.Models;

/// <summary>JSON language pack. <c>cultureCode</c> plus <c>localizedStrings</c>.</summary>
public sealed class LanguagePackDocument
{
    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string? CultureCode { get; set; }

    /// <summary>
    /// Gets or sets the localized strings.
    /// </summary>
    public Dictionary<string, string> LocalizedStrings { get; set; } = new(StringComparer.Ordinal);
}
