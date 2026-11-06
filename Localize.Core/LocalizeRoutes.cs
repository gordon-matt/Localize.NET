namespace Localize;

/// <summary>Routes the admin pages and the culture endpoint share.</summary>
public static class LocalizeRoutes
{
    /// <summary>
    /// The languages constant.
    /// </summary>
    public const string Languages = "/localize/languages";

    /// <summary>
    /// The strings constant.
    /// </summary>
    public const string Strings = "/localize/strings";

    /// <summary>
    /// The export constant.
    /// </summary>
    public const string Export = "/localize/export";

    /// <summary>
    /// The culture constant.
    /// </summary>
    public const string Culture = "/localize/culture";

    /// <summary>
    /// Gets the strings for.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <returns>The strings for.</returns>
    public static string StringsFor(string? cultureCode) =>
        string.IsNullOrEmpty(cultureCode) ? Strings : $"{Strings}/{cultureCode}";
}
