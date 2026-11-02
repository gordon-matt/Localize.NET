namespace Localize.Models;

/// <summary>JSON language pack. <c>cultureCode</c> plus <c>localizedStrings</c>.</summary>
public sealed class LanguagePackDocument
{
    public string? CultureCode { get; set; }

    public Dictionary<string, string> LocalizedStrings { get; set; } = new(StringComparer.Ordinal);
}
