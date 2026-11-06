using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Localize.Data.Npgsql;

/// <summary>
/// Provides extension methods for service collections.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Stores languages and strings in PostgreSQL.</summary>
        public IServiceCollection AddLocalizeNpgsql(string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.RemoveAll<ILocalizeDbContextFactory>();
            services.AddSingleton<ILocalizeDbContextFactory>(new LocalizeDbContextFactory(connectionString));
            LocalizeProviderRegistry.Attach?.Invoke(services);
            return services;
        }
    }
}
