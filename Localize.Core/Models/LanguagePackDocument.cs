namespace Localize.Models;

/// <summary>JSON language pack. Property names match the file Inferno imports and exports.</summary>
public sealed class LanguagePackDocument
{
    public string? CultureCode { get; set; }

    public Dictionary<string, string> LocalizedStrings { get; set; } = new(StringComparer.Ordinal);
}
