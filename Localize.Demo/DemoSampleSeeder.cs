using Localize.Languages;
using Localize.Models;
using Localize.Strings;
using Microsoft.Extensions.Hosting;

namespace Localize.Demo;

/// <summary>
/// Enables French and neutral French, and stores one database row so the home page
/// can show priority 100 winning over the files. An existing row is left as the editor saved it.
/// </summary>
internal sealed class DemoSampleSeeder(IServiceScopeFactory scopes, ILogger<DemoSampleSeeder> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopes.CreateScope();
        var languages = scope.ServiceProvider.GetRequiredService<ILanguageService>();
        var strings = scope.ServiceProvider.GetRequiredService<ILocalizableStringService>();

        await EnsureLanguageAsync(languages, "fr", "French", sortOrder: 1, cancellationToken);
        await EnsureLanguageAsync(languages, "fr-FR", "French (France)", sortOrder: 2, cancellationToken);

        await EnsureStringAsync(strings, null, "Demo/Only.Database", "Hello from the database.", cancellationToken);
        await EnsureStringAsync(strings, "fr-FR", "Demo/Only.Database", "Bonjour depuis la base de données.", cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureLanguageAsync(
        ILanguageService languages,
        string culture,
        string name,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var listed = await languages.ListAsync(cancellationToken);
        if (!listed.IsSuccess)
        {
            logger.LogWarning("Could not list languages while seeding the demo.");
            return;
        }

        var existing = listed.Value.FirstOrDefault(language =>
            language.CultureCode.Equals(culture, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            if (culture == "fr-FR" && existing.Name == "French")
            {
                await languages.SaveAsync(new LanguageForm
                {
                    Id = existing.Id,
                    Name = name,
                    CultureCode = existing.CultureCode,
                    IsRightToLeft = existing.IsRightToLeft,
                    IsEnabled = existing.IsEnabled,
                    SortOrder = existing.SortOrder,
                }, cancellationToken);
            }

            return;
        }

        var saved = await languages.SaveAsync(new LanguageForm
        {
            Name = name,
            CultureCode = culture,
            IsEnabled = true,
            SortOrder = sortOrder,
        }, cancellationToken);

        if (!saved.IsSuccess)
        {
            logger.LogWarning("Could not add the demo language {Culture}.", culture);
        }
    }

    private async Task EnsureStringAsync(
        ILocalizableStringService strings,
        string? culture,
        string key,
        string value,
        CancellationToken cancellationToken)
    {
        var listed = await strings.ListComparativeAsync(culture, key, cancellationToken);
        if (!listed.IsSuccess)
        {
            logger.LogWarning("Could not read strings while seeding {Key}.", key);
            return;
        }

        if (listed.Value.Any(item => item.Key == key && item.IsStored))
        {
            return;
        }

        var saved = await strings.SaveAsync(culture, key, value, cancellationToken);
        if (!saved.IsSuccess)
        {
            logger.LogWarning("Could not store the demo string {Key}.", key);
        }
    }
}
