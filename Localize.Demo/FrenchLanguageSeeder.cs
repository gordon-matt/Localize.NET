using Localize.Languages;
using Localize.Models;
using Microsoft.Extensions.Hosting;

namespace Localize.Demo;

/// <summary>The sample JSON file is French, so the switcher needs that language enabled.</summary>
internal sealed class FrenchLanguageSeeder(IServiceScopeFactory scopes) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopes.CreateScope();
        var languages = scope.ServiceProvider.GetRequiredService<ILanguageService>();
        var listed = await languages.ListAsync(cancellationToken);
        if (!listed.IsSuccess || listed.Value.Any(language => language.CultureCode == "fr-FR"))
        {
            return;
        }

        await languages.SaveAsync(new LanguageForm
        {
            Name = "French",
            CultureCode = "fr-FR",
            IsEnabled = true,
            SortOrder = 1,
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
