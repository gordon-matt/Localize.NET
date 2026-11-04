namespace Localize.Tests;

public class CultureCodesTests
{
    [Fact]
    public void Fallback_walks_parents_then_invariant()
    {
        var chain = CultureCodes.FallbackChain("en-AU", includeParents: true);

        Assert.Equal(["en-AU", "en", ""], chain);
    }

    [Fact]
    public void Normalize_canonicalizes_and_treats_blank_as_invariant()
    {
        Assert.Equal("", CultureCodes.Normalize(null));
        Assert.Equal("", CultureCodes.Normalize(" invariant "));
        Assert.Equal("fr-FR", CultureCodes.Normalize("fr-fr"));
    }

    [Fact]
    public void NeutralCultures_ListsLanguagesWithoutRegions()
    {
        var neutral = CultureCodes.NeutralCultures();

        Assert.Contains(neutral, choice => choice.Code == "fr");
        Assert.DoesNotContain(neutral, choice => choice.Code.Contains('-'));
        Assert.DoesNotContain(neutral, choice => choice.Code.Length == 0);
    }

    [Fact]
    public void SpecificCulturesOf_Language_ListsItsRegions()
    {
        var regions = CultureCodes.SpecificCulturesOf("fr");

        Assert.Contains(regions, choice => choice.Code == "fr-CA");
        Assert.All(regions, choice => Assert.Equal("fr", CultureCodes.RootNeutral(choice.Code)));
    }

    [Fact]
    public void SpecificCulturesOf_LanguageWithScripts_IncludesRegionsUnderTheScript()
    {
        var regions = CultureCodes.SpecificCulturesOf("zh");

        Assert.Contains(regions, choice => choice.Code.StartsWith("zh-", StringComparison.Ordinal) && choice.Code.EndsWith("-CN", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("fr-CA", "fr")]
    [InlineData("fr", "fr")]
    [InlineData("zh-CN", "zh")]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void RootNeutral_Culture_ReturnsItsLanguage(string? code, string? expected)
    {
        Assert.Equal(expected, CultureCodes.RootNeutral(code));
    }

    [Fact]
    public void SpecificCulturesOf_UnknownCode_IsEmpty()
    {
        Assert.Empty(CultureCodes.SpecificCulturesOf("not-a-culture-at-all"));
    }

    [Fact]
    public void Fallback_without_parents_is_the_culture_and_invariant()
    {
        var chain = CultureCodes.FallbackChain("fr-FR", includeParents: false);

        Assert.Equal(["fr-FR", ""], chain);
    }
}