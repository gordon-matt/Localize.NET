using System.Resources;

namespace Localize.Demo;

/// <summary>
/// Accessor for <c>Resources/DemoResources.resx</c> and its culture satellites.
/// <c>AddLocalizeResx</c> reads the public static <see cref="ResourceManager"/>.
/// </summary>
public sealed class DemoResources
{
    public static ResourceManager ResourceManager { get; } = new(
        "Localize.Demo.Resources.DemoResources",
        typeof(DemoResources).Assembly);
}
