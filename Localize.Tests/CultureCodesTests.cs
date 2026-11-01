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
    public void Fallback_without_parents_is_the_culture_and_invariant()
    {
        var chain = CultureCodes.FallbackChain("fr-FR", includeParents: false);

        Assert.Equal(["fr-FR", ""], chain);
    }
}
