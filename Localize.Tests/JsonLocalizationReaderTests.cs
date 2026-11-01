namespace Localize.Tests;

public class JsonLocalizationReaderTests
{
    [Fact]
    public void Reads_an_inferno_language_pack()
    {
        const string json = """
            {
              "CultureCode": "fr-FR",
              "LocalizedStrings": {
                "App/Home.Title": "Accueil"
              }
            }
            """;

        Assert.True(JsonLocalizationReader.TryRead(json, out string? culture, out var strings, out string? error));
        Assert.Null(error);
        Assert.Equal("fr-FR", culture);
        Assert.Equal("Accueil", strings["App/Home.Title"]);
    }

    [Fact]
    public void Flattens_nested_objects_with_dots()
    {
        const string json = """
            {
              "Home": {
                "Title": "Hello"
              }
            }
            """;

        Assert.True(JsonLocalizationReader.TryRead(json, out string? culture, out var strings, out _));
        Assert.Null(culture);
        Assert.Equal("Hello", strings["Home.Title"]);
    }

    [Fact]
    public void Culture_comes_from_a_parent_folder_or_the_file_name()
    {
        Assert.Equal("fr-FR", JsonLocalizationReader.CultureFromPath("Localization/fr-FR/Home.json"));
        Assert.Equal("fr-FR", JsonLocalizationReader.CultureFromPath("Localization/fr-fr.json"));
        Assert.Equal("", JsonLocalizationReader.CultureFromPath("Localization/default.json"));
        Assert.Null(JsonLocalizationReader.CultureFromPath("Localization/Home.json"));
    }
}
