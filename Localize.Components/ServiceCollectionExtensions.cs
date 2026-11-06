using Heroicsoft.NetForge.Components;
using Heroicsoft.NetForge.Components.Toasts;

namespace Localize.Components;

/// <summary>
/// Provides extension methods for service collections.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the Blazor admin and its menu contributor. Pass the same options here or call
        /// <c>AddLocalize</c> yourself first. Add a database with <c>AddLocalizeSqlite</c> or one of
        /// its siblings, and add this assembly to <c>ForgeRouter.AdditionalAssemblies</c>.
        /// </summary>
        public IServiceCollection AddLocalizeComponents(Action<LocalizeOptions>? configure = null)
        {
            if (!services.Any(descriptor => descriptor.ServiceType == typeof(ToastService)))
            {
                services.AddNetForgeComponents();
            }

            services.AddLocalize(configure);
            services.AddNetForgeMenuContributor<LocalizeMenuContributor>();
            return services;
        }
    }
}
