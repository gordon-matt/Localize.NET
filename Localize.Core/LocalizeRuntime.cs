using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Localize;

/// <summary>
/// Holds the built service provider so display attributes can resolve <see cref="IStringLocalizer"/>
/// without a service locator in every model. Set when the host starts.
/// </summary>
public static class LocalizeRuntime
{
    public static IServiceProvider? Services { get; set; }

    public static string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return key;
        }

        var provider = Services;
        if (provider is null)
        {
            return key;
        }

        try
        {
            var localizer = provider.GetService<IStringLocalizer>();
            if (localizer is null)
            {
                using var scope = provider.CreateScope();
                localizer = scope.ServiceProvider.GetService<IStringLocalizer>();
            }

            return localizer is null ? key : localizer[key].Value;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return key;
        }
    }
}
