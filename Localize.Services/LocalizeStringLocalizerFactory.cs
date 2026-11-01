using Microsoft.Extensions.Localization;

namespace Localize;

public sealed class LocalizeStringLocalizerFactory(IServiceProvider provider) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) => provider.GetRequiredService<IStringLocalizer>();

    public IStringLocalizer Create(string baseName, string location) => provider.GetRequiredService<IStringLocalizer>();
}

/// <summary>
/// <see cref="IStringLocalizer{T}"/> shares the one catalog. Resource classes are not prefixes;
/// keys stay the flat keys a language pack already uses.
/// </summary>
public sealed class StringLocalizerAdapter<T>(IStringLocalizer inner) : IStringLocalizer<T>
{
    public LocalizedString this[string name] => inner[name];

    public LocalizedString this[string name, params object[] arguments] => inner[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        inner.GetAllStrings(includeParentCultures);
}
