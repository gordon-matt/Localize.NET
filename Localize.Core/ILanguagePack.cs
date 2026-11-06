namespace Localize;

/// <summary>
/// Default strings shipped with the application. Register every pack in dependency injection.
/// Leave <see cref="CultureCode"/> null for the invariant culture.
/// </summary>
public interface ILanguagePack
{
    /// <summary>Null or empty for the invariant culture.</summary>
    string? CultureCode { get; }

    /// <summary>
    /// Gets the localized strings.
    /// </summary>
    IDictionary<string, string> LocalizedStrings { get; }
}
