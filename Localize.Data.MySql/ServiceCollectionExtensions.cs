using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Localize.Data.MySql;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Stores languages and strings in MySQL.</summary>
        public IServiceCollection AddLocalizeMySql(string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.RemoveAll<ILocalizeDbContextFactory>();
            services.AddSingleton<ILocalizeDbContextFactory>(new LocalizeDbContextFactory(connectionString));
            LocalizeProviderRegistry.Attach?.Invoke(services);
            return services;
        }
    }
}
