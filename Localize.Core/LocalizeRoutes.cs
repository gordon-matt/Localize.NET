namespace Localize;

/// <summary>Routes the admin pages and the culture endpoint share.</summary>
public static class LocalizeRoutes
{
    public const string Languages = "/localize/languages";

    public const string Strings = "/localize/strings";

    public const string Export = "/localize/export";

    public const string Culture = "/localize/culture";

    public static string StringsFor(string? cultureCode) =>
        string.IsNullOrEmpty(cultureCode) ? Strings : $"{Strings}/{cultureCode}";
}
