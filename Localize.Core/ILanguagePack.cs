namespace Localize;

/// <summary>
/// Default strings shipped with the application. Register every pack in dependency injection.
/// Leave <see cref="CultureCode"/> null for the invariant culture. The same shape Inferno uses,
/// so an existing pack class only changes its namespace.
/// </summary>
public interface ILanguagePack
{
    /// <summary>Null or empty for the invariant culture.</summary>
    string? CultureCode { get; }

    IDictionary<string, string> LocalizedStrings { get; }
}
