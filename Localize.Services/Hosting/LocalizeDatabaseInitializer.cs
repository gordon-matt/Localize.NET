using Microsoft.Extensions.Hosting;

namespace Localize.Hosting;

internal sealed class LocalizeDatabaseInitializer(
    ILogger<LocalizeDatabaseInitializer> logger,
    IOptions<LocalizeOptions> options,
    ILocalizeDbContextFactory contextFactory,
    IRepository<Language> languages) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.Value.AutoMigrate)
        {
            try
            {
                using var context = contextFactory.GetContext();
                await context.Database.MigrateAsync(cancellationToken);
                logger.LogInformation("Localize database is up to date ({Provider}).", contextFactory.ProviderName);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Could not migrate the Localize database ({Provider}).", contextFactory.ProviderName);
                return;
            }
        }

        if (!options.Value.SeedDefaultLanguage || !CultureCodes.IsKnown(options.Value.DefaultCulture))
        {
            return;
        }

        try
        {
            string culture = CultureCodes.Normalize(options.Value.DefaultCulture);
            bool exists = await languages.ExistsAsync(
                language => language.ScopeId == CultureCodes.GlobalScope && language.CultureCode == culture,
                ContextOptions.ForCancellationToken(cancellationToken));

            if (exists)
            {
                return;
            }

            await languages.InsertAsync(new Language
            {
                ScopeId = CultureCodes.GlobalScope,
                CultureCode = culture,
                Name = CultureCodes.DisplayName(culture),
                IsRightToLeft = CultureCodes.IsRightToLeft(culture),
                IsEnabled = true,
                SortOrder = 0,
            }, ContextOptions.ForCancellationToken(cancellationToken));

            logger.LogInformation("Seeded the default language {Culture}.", culture);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Could not seed the default language.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
