using Localize.Data;
using Localize.Data.Sqlite;
using Localize.Languages;
using Localize.Models;
using Localize.Properties;
using Localize.Strings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Localize.Tests;

public class LocalizationServiceTests
{
    [Fact]
    public async Task Database_overrides_a_language_pack_and_formats_arguments()
    {
        await using var database = new TestDatabase();
        using var scope = database.Services.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer>();
        var strings = scope.ServiceProvider.GetRequiredService<ILocalizableStringService>();

        using (new CultureSwitch("en-AU"))
        {
            Assert.Equal("Hello", localizer["App/Home.Title"].Value);
            Assert.Equal("Hello Ada", localizer["App/Home.Greeting", "Ada"].Value);
        }

        var saved = await strings.SaveAsync("en-AU", "App/Home.Title", "G'day");
        Assert.True(saved.IsSuccess);

        using (new CultureSwitch("en-AU"))
        {
            Assert.Equal("G'day", localizer["App/Home.Title"].Value);
        }
    }

    [Fact]
    public async Task Property_translation_falls_back_to_the_entity_value()
    {
        await using var database = new TestDatabase();
        using var scope = database.Services.CreateScope();
        var properties = scope.ServiceProvider.GetRequiredService<ILocalizablePropertyService>();

        var saved = await properties.SaveAsync(new LocalizablePropertyForm
        {
            CultureCode = "fr-FR",
            EntityType = "Demo.Page",
            EntityId = "1",
            Property = "Title",
            Value = "Accueil",
        });
        Assert.True(saved.IsSuccess);

        Assert.Equal("Accueil", await properties.ValueOrDefaultAsync("fr-FR", "Demo.Page", "1", "Title", "Home"));
        Assert.Equal("Home", await properties.ValueOrDefaultAsync("de-DE", "Demo.Page", "1", "Title", "Home"));
        Assert.Equal("Home", await properties.ValueOrDefaultAsync(null, "Demo.Page", "1", "Title", "Home"));
    }

    [Fact]
    public async Task Editor_lists_pack_keys_and_marks_a_saved_override()
    {
        await using var database = new TestDatabase();
        using var scope = database.Services.CreateScope();
        var strings = scope.ServiceProvider.GetRequiredService<ILocalizableStringService>();

        var listed = await strings.ListComparativeAsync("fr-FR", search: null);
        Assert.True(listed.IsSuccess);
        var title = Assert.Single(listed.Value, row => row.Key == "App/Home.Title");
        Assert.Equal("Hello", title.InvariantValue);
        Assert.Null(title.LocalizedValue);
        Assert.False(title.IsStored);

        var saved = await strings.SaveAsync("fr-FR", "App/Home.Title", "Bonjour");
        Assert.True(saved.IsSuccess);

        listed = await strings.ListComparativeAsync("fr-FR", search: null);
        title = Assert.Single(listed.Value, row => row.Key == "App/Home.Title");
        Assert.Equal("Bonjour", title.LocalizedValue);
        Assert.True(title.IsStored);
    }

    [Fact]
    public async Task Import_accepts_an_inferno_pack_and_adds_the_language_switched_off()
    {
        await using var database = new TestDatabase();
        using var scope = database.Services.CreateScope();
        var strings = scope.ServiceProvider.GetRequiredService<ILocalizableStringService>();
        var languages = scope.ServiceProvider.GetRequiredService<ILanguageService>();

        const string json = """
            {
              "CultureCode": "fr-FR",
              "LocalizedStrings": { "App/Home.Title": "Accueil" }
            }
            """;

        var imported = await strings.ImportAsync(json);
        Assert.True(imported.IsSuccess);
        Assert.True(imported.Value.LanguageCreated);
        Assert.Equal(1, imported.Value.Added);

        var listed = await languages.ListAsync();
        var french = Assert.Single(listed.Value, language => language.CultureCode == "fr-FR");
        Assert.False(french.IsEnabled);
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), "localize-tests", Guid.NewGuid().ToString("N"));
        private readonly ServiceProvider provider;

        public TestDatabase()
        {
            Directory.CreateDirectory(directory);
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            services.AddLocalize(options =>
            {
                options.AutoMigrate = false;
                options.SeedDefaultLanguage = false;
                options.TrackMissingKeys = false;
            });
            services.AddSingleton<ILanguagePack, SamplePack>();
            services.AddLocalizeSqlite($"Data Source={Path.Combine(directory, "localize.db")}");
            provider = services.BuildServiceProvider();

            using var context = provider.GetRequiredService<ILocalizeDbContextFactory>().GetContext();
            context.Database.EnsureCreated();
        }

        public IServiceProvider Services => provider;

        public async ValueTask DisposeAsync()
        {
            await provider.DisposeAsync();
            try
            {
                Directory.Delete(directory, recursive: true);
            }
            catch (IOException)
            {
                // SQLite can keep the file open for a moment. The temp folder is unique.
            }
        }
    }

    private sealed class SamplePack : ILanguagePack
    {
        public string? CultureCode => null;

        public IDictionary<string, string> LocalizedStrings { get; } = new Dictionary<string, string>
        {
            ["App/Home.Title"] = "Hello",
            ["App/Home.Greeting"] = "Hello {0}",
        };
    }

    private sealed class CultureSwitch : IDisposable
    {
        private readonly CultureInfo previous = CultureInfo.CurrentUICulture;

        public CultureSwitch(string culture) => CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

        public void Dispose() => CultureInfo.CurrentUICulture = previous;
    }
}
