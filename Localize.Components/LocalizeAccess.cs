using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Localize.Components;

/// <summary>Admin pages fail closed when authentication is required and the host has no auth services.</summary>
public static class LocalizeAccess
{
    public static async Task<bool> AllowAsync(LocalizeOptions options, IServiceProvider services)
    {
        if (!options.RequireAuthentication && string.IsNullOrWhiteSpace(options.ManagePolicy))
        {
            return true;
        }

        var authState = services.GetService<AuthenticationStateProvider>();
        if (authState is null)
        {
            return false;
        }

        var state = await authState.GetAuthenticationStateAsync();
        if (!string.IsNullOrWhiteSpace(options.ManagePolicy))
        {
            var authorization = services.GetService<IAuthorizationService>();
            if (authorization is null)
            {
                return false;
            }

            return (await authorization.AuthorizeAsync(state.User, options.ManagePolicy)).Succeeded;
        }

        return state.User.Identity?.IsAuthenticated == true;
    }
}
