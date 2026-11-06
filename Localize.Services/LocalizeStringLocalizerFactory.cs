using Microsoft.Extensions.Localization;

namespace Localize;

/// <summary>
/// Represents the localize string localizer factory.
/// </summary>
public sealed class LocalizeStringLocalizerFactory(IServiceProvider provider) : IStringLocalizerFactory
{
    /// <summary>
    /// Creates the value.
    /// </summary>
    /// <param name="resourceSource">The resource source.</param>
    /// <returns>The i string localizer.</returns>
    public IStringLocalizer Create(Type resourceSource) => provider.GetRequiredService<IStringLocalizer>();

    /// <summary>
    /// Creates the value.
    /// </summary>
    /// <param name="baseName">The base name.</param>
    /// <param name="location">The location.</param>
    /// <returns>The i string localizer.</returns>
    public IStringLocalizer Create(string baseName, string location) => provider.GetRequiredService<IStringLocalizer>();
}

/// <summary>
/// <see cref="IStringLocalizer{T}"/> shares the one catalog. Resource classes are not prefixes;
/// keys stay the flat keys a language pack already uses.
/// </summary>
public sealed class StringLocalizerAdapter<T>(IStringLocalizer inner) : IStringLocalizer<T>
{
    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The item at the specified index.</returns>
    public LocalizedString this[string name] => inner[name];

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The item at the specified index.</returns>
    public LocalizedString this[string name, params object[] arguments] => inner[name, arguments];

    /// <summary>
    /// Gets the all strings.
    /// </summary>
    /// <param name="includeParentCultures">The include parent cultures.</param>
    /// <returns>The all strings.</returns>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        inner.GetAllStrings(includeParentCultures);
}
