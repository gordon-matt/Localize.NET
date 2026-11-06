using Microsoft.AspNetCore.Localization;

namespace Localize.Components;

/// <summary>Reads the culture cookie named in <see cref="LocalizeOptions.CultureCookieName"/>.</summary>
public sealed class LocalizeCookieCultureProvider : RequestCultureProvider
{
    /// <inheritdoc/>
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var options = httpContext.RequestServices.GetRequiredService<IOptions<LocalizeOptions>>().Value;
        if (httpContext.Request.Cookies.TryGetValue(options.CultureCookieName, out string? value)
            && CultureCodes.IsKnown(value))
        {
            string culture = CultureCodes.Normalize(value);
            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture, culture));
        }

        return Task.FromResult<ProviderCultureResult?>(null);
    }
}
