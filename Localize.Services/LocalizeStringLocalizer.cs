using Microsoft.Extensions.Localization;

namespace Localize;

internal sealed class LocalizeStringLocalizer(
    ILocalizationCatalog catalog,
    ILocalizationScope scope,
    IOptions<LocalizeOptions> options,
    IMissingKeyTracker tracker) : IStringLocalizer
{
    public LocalizedString this[string name] => Find(name);

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var found = Find(name);
            if (found.ResourceNotFound || arguments is not { Length: > 0 })
            {
                return found;
            }

            try
            {
                string formatted = string.Format(CultureInfo.CurrentUICulture, found.Value, arguments);
                return new LocalizedString(name, formatted, resourceNotFound: false);
            }
            catch (FormatException)
            {
                return found;
            }
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var all = catalog.GetAll(scope.GetScopeId(), CultureInfo.CurrentUICulture.Name, includeParentCultures);
        return all.Select(pair => new LocalizedString(pair.Key, pair.Value, resourceNotFound: false));
    }

    private LocalizedString Find(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new LocalizedString(name ?? "", name ?? "", resourceNotFound: true);
        }

        string? scopeId = scope.GetScopeId();
        string? value = catalog.GetString(scopeId, CultureInfo.CurrentUICulture.Name, name);
        if (value is not null)
        {
            return new LocalizedString(name, value, resourceNotFound: false);
        }

        if (options.Value.TrackMissingKeys)
        {
            tracker.Track(CultureCodes.NormalizeScope(scopeId), name);
        }

        return new LocalizedString(name, name, resourceNotFound: true);
    }
}
