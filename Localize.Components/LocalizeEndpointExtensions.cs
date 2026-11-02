using System.Text;
using System.Text.Json;
using Localize.Languages;
using Localize.Strings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace Localize.Components;

public static class LocalizeEndpointExtensions
{
    /// <summary>Maps the culture switch and the language-pack download.</summary>
    public static IEndpointRouteBuilder MapLocalize(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{LocalizeRoutes.Culture}/{{cultureCode}}", SetCultureAsync);
        endpoints.MapGet($"{LocalizeRoutes.Export}/{{cultureCode}}", ExportAsync);
        return endpoints;
    }

    private static async Task<HttpResult> SetCultureAsync(
        string cultureCode,
        string? returnUrl,
        HttpContext http,
        IOptions<LocalizeOptions> options)
    {
        if (!CultureCodes.IsKnown(cultureCode))
        {
            return Results.BadRequest();
        }

        string culture = CultureCodes.Normalize(cultureCode);
        var languages = http.RequestServices.GetService<ILanguageService>();
        if (languages is not null)
        {
            var enabled = await languages.ListEnabledAsync(http.RequestAborted);
            if (!enabled.IsSuccess || enabled.Value.All(language => language.CultureCode != culture))
            {
                return Results.BadRequest();
            }
        }

        http.Response.Cookies.Append(options.Value.CultureCookieName, culture, new CookieOptions
        {
            Path = "/",
            IsEssential = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = http.Request.IsHttps,
            MaxAge = TimeSpan.FromDays(365),
        });

        return Results.LocalRedirect(LocalReturn(returnUrl));
    }

    private static async Task<HttpResult> ExportAsync(
        string cultureCode,
        HttpContext http,
        IOptions<LocalizeOptions> options,
        ILocalizableStringService strings)
    {
        if (!await LocalizeAccess.AllowAsync(options.Value, http.RequestServices, http.User))
        {
            return Results.Challenge();
        }

        var exported = await strings.ExportAsync(cultureCode, http.RequestAborted);
        if (!exported.IsSuccess)
        {
            return Results.NotFound();
        }

        byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(exported.Value, LocalizeJson.Options));
        string culture = CultureCodes.IsInvariant(cultureCode) ? "invariant" : CultureCodes.Normalize(cultureCode);
        string fileName = $"LanguagePack_{culture}_{DateTime.UtcNow:yyyy-MM-dd}.json";
        return Results.File(body, "application/json", fileName);
    }

    private static string LocalReturn(string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)
            || !returnUrl.StartsWith('/')
            || returnUrl.StartsWith("//", StringComparison.Ordinal)
            || returnUrl.Contains('\\', StringComparison.Ordinal))
        {
            return "/";
        }

        return returnUrl;
    }
}
