using System.Reflection;
using System.Resources;
using Localize.Data;
using Localize.Hosting;
using Localize.Languages;
using Localize.Properties;
using Localize.Scope;
using Localize.Strings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Localize;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the string localizer, JSON and RESX sources, and language packs.
        /// Add a database as well, for example <c>AddLocalizeSqlite()</c>. Either call can come first.
        /// Register <see cref="ILocalizationScope"/> before this, or call <see cref="AddLocalize{TScope}"/>.
        /// </summary>
        public IServiceCollection AddLocalize(Action<LocalizeOptions>? configure = null)
        {
            var builder = services.AddOptions<LocalizeOptions>();
            if (services.Any(descriptor => descriptor.ServiceType == typeof(IConfiguration)))
            {
                builder.BindConfiguration(LocalizeOptions.SectionName);
            }

            if (configure is not null)
            {
                builder.Configure(configure);
            }

            LocalizeProviderRegistry.Attach = RegisterData;

            if (!services.Any(descriptor => descriptor.ServiceType == typeof(LocalizeCoreMarker)))
            {
                RegisterCore(services);
            }

            if (services.Any(descriptor => descriptor.ServiceType == typeof(ILocalizeDbContextFactory)))
            {
                RegisterData(services);
            }

            return services;
        }

        /// <summary>Same as <see cref="AddLocalize"/>, using <typeparamref name="TScope"/> as the current tenant.</summary>
        public IServiceCollection AddLocalize<TScope>(Action<LocalizeOptions>? configure = null)
            where TScope : class, ILocalizationScope
        {
            services.AddScoped<ILocalizationScope, TScope>();
            return services.AddLocalize(configure);
        }

        /// <summary>Reads strings from a RESX <see cref="ResourceManager"/>. Database rows still override them.</summary>
        public IServiceCollection AddLocalizeResx(ResourceManager manager)
        {
            ArgumentNullException.ThrowIfNull(manager);
            services.AddSingleton(new ResxResource(manager));
            return services;
        }

        /// <summary>
        /// Reads strings from a generated RESX class, the kind with a public static
        /// <c>ResourceManager</c> property.
        /// </summary>
        public IServiceCollection AddLocalizeResx<TResource>()
            where TResource : class
        {
            var property = typeof(TResource).GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static);
            if (property?.GetValue(null) is not ResourceManager manager)
            {
                throw new InvalidOperationException(
                    $"{typeof(TResource).FullName} does not expose a public static ResourceManager.");
            }

            return services.AddLocalizeResx(manager);
        }
    }

    private static void RegisterCore(IServiceCollection services)
    {
        services.AddSingleton<LocalizeCoreMarker>();
        services.TryAddScoped<ILocalizationScope, NullLocalizationScope>();
        services.AddMemoryCache();
        services.TryAddSingleton<ILocalizationCache, LocalizationCache>();

        services.AddSingleton<LanguagePackSource>();
        services.AddSingleton<ILocalizationSource>(static provider => provider.GetRequiredService<LanguagePackSource>());
        services.AddSingleton<JsonLocalizationSource>();
        services.AddSingleton<ILocalizationSource>(static provider => provider.GetRequiredService<JsonLocalizationSource>());
        services.AddSingleton<ResxLocalizationSource>();
        services.AddSingleton<ILocalizationSource>(static provider => provider.GetRequiredService<ResxLocalizationSource>());

        services.TryAddSingleton<ILocalizationCatalog, LocalizationCatalog>();
        services.TryAddSingleton<IMissingKeyTracker, NullMissingKeyTracker>();

        services.Replace(ServiceDescriptor.Scoped<IStringLocalizer, LocalizeStringLocalizer>());
        services.Replace(ServiceDescriptor.Scoped(typeof(IStringLocalizer<>), typeof(StringLocalizerAdapter<>)));
        services.Replace(ServiceDescriptor.Scoped<IStringLocalizerFactory, LocalizeStringLocalizerFactory>());

        services.AddHostedService<LocalizeRuntimeInitializer>();
        services.AddHostedService<JsonFileWatchService>();
    }

    private static void RegisterData(IServiceCollection services)
    {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(LocalizeDataMarker)))
        {
            return;
        }

        if (!services.Any(descriptor => descriptor.ServiceType == typeof(ILocalizeDbContextFactory)))
        {
            throw new InvalidOperationException(
                "Register a Localize database before the storage services. Call AddLocalizeSqlite, AddLocalizeSqlServer, AddLocalizeNpgsql, or AddLocalizeMySql.");
        }

        services.AddSingleton<LocalizeDataMarker>();
        AddRepository<Language>(services);
        AddRepository<LocalizableString>(services);
        AddRepository<LocalizableProperty>(services);

        services.AddSingleton<DatabaseLocalizationSource>();
        services.AddSingleton<ILocalizationSource>(static provider => provider.GetRequiredService<DatabaseLocalizationSource>());
        services.Replace(ServiceDescriptor.Singleton<IMissingKeyTracker, DatabaseMissingKeyTracker>());

        services.TryAddScoped<ILanguageService, LanguageService>();
        services.TryAddScoped<ILocalizableStringService, LocalizableStringService>();
        services.TryAddScoped<ILocalizablePropertyService, LocalizablePropertyService>();
        services.AddHostedService<LocalizeDatabaseInitializer>();
    }

    private static void AddRepository<TEntity>(IServiceCollection services)
        where TEntity : class, IEntity
    {
        services.Replace(ServiceDescriptor.Singleton<IRepository<TEntity>>(static provider =>
            new EntityFrameworkRepository<TEntity>(provider.GetRequiredService<ILocalizeDbContextFactory>())));
    }

    private sealed class LocalizeCoreMarker;

    private sealed class LocalizeDataMarker;
}
