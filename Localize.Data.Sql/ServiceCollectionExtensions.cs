using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Localize.Data.Sql;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Stores languages and strings in SQL Server.</summary>
        public IServiceCollection AddLocalizeSqlServer(string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.RemoveAll<ILocalizeDbContextFactory>();
            services.AddSingleton<ILocalizeDbContextFactory>(new LocalizeDbContextFactory(connectionString));
            LocalizeProviderRegistry.Attach?.Invoke(services);
            return services;
        }
    }
}
