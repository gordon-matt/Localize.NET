namespace Localize;

/// <summary>
/// Splits languages and strings per tenant. Return null for a single-tenant site.
/// Register it before <c>AddLocalize</c>, or call <c>AddLocalize&lt;TScope&gt;()</c>.
/// </summary>
public interface ILocalizationScope
{
    /// <summary>
    /// Gets the scope identifier.
    /// </summary>
    /// <returns>The scope identifier.</returns>
    string? GetScopeId();
}
