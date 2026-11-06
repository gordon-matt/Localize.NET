using Microsoft.AspNetCore.Localization;

namespace Localize.Components;

/// <summary>
/// Provides extension methods for localize application builders.
/// </summary>
public static class LocalizeApplicationBuilderExtensions
{
    /// <summary>
    /// Sets the request culture from the Localize cookie, then from the browser's Accept-Language
    /// header, then from <see cref="LocalizeOptions.DefaultCulture"/>. Call this instead of
    /// <c>UseRequestLocalization</c>.
    /// </summary>
    public static IApplicationBuilder UseLocalize(this IApplicationBuilder app)
    {
        var localize = app.ApplicationServices.GetRequiredService<IOptions<LocalizeOptions>>().Value;
        string fallback = CultureCodes.IsKnown(localize.DefaultCulture)
            ? CultureCodes.Normalize(localize.DefaultCulture)
            : "en-US";

        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(culture => !string.IsNullOrEmpty(culture.Name))
            .ToList();

        var localization = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(fallback),
            SupportedCultures = cultures,
            SupportedUICultures = cultures,
        };
        localization.RequestCultureProviders.Clear();
        localization.RequestCultureProviders.Add(new LocalizeCookieCultureProvider());
        localization.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

        app.UseRequestLocalization(localization);
        return app;
    }
}
