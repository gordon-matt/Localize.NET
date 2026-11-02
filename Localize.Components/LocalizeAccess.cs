using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Localize.Components;

/// <summary>Admin pages fail closed when authentication is required and the host has no auth services.</summary>
public static class LocalizeAccess
{
    /// <summary>Used by the Blazor pages, which own an authentication-state provider.</summary>
    public static Task<bool> AllowAsync(LocalizeOptions options, IServiceProvider services) =>
        AllowAsync(options, services, user: null);

    /// <summary>
    /// Pass <paramref name="user"/> from an HTTP endpoint. The Blazor authentication-state provider
    /// throws when it is resolved outside a component.
    /// </summary>
    public static async Task<bool> AllowAsync(LocalizeOptions options, IServiceProvider services, ClaimsPrincipal? user)
    {
        if (!options.RequireAuthentication && string.IsNullOrWhiteSpace(options.ManagePolicy))
        {
            return true;
        }

        user ??= await UserFromCircuitAsync(services);
        if (user is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.ManagePolicy))
        {
            var authorization = services.GetService<IAuthorizationService>();
            if (authorization is null)
            {
                return false;
            }

            return (await authorization.AuthorizeAsync(user, options.ManagePolicy)).Succeeded;
        }

        return user.Identity?.IsAuthenticated == true;
    }

    private static async Task<ClaimsPrincipal?> UserFromCircuitAsync(IServiceProvider services)
    {
        var authState = services.GetService<AuthenticationStateProvider>();
        if (authState is null)
        {
            return null;
        }

        var state = await authState.GetAuthenticationStateAsync();
        return state.User;
    }
}
