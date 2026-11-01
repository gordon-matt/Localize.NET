using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Localize.Data.Sqlite;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Stores languages and strings in SQLite, and attaches the storage services when
        /// <c>AddLocalize</c> has run. Either call can come first.
        /// </summary>
        /// <param name="connectionString">
        /// A SQLite connection string. When omitted, data goes to <c>Localize.db</c> under the
        /// user's local application data folder.
        /// </param>
        public IServiceCollection AddLocalizeSqlite(string? connectionString = null)
        {
            string resolved = string.IsNullOrWhiteSpace(connectionString)
                ? $"Data Source={DefaultDatabasePath()}"
                : connectionString;

            services.RemoveAll<ILocalizeDbContextFactory>();
            services.AddSingleton<ILocalizeDbContextFactory>(new LocalizeDbContextFactory(resolved));
            LocalizeProviderRegistry.Attach?.Invoke(services);
            return services;
        }
    }

    private static string DefaultDatabasePath()
    {
        string appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Localize";

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            appName,
            "Localize.db");
    }
}
