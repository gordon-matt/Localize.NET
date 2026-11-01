namespace Localize.Tracking;

/// <summary>Stores a key the first time a page asks for it, so an editor can translate it.</summary>
internal sealed class DatabaseMissingKeyTracker(
    IRepository<LocalizableString> strings,
    ILocalizationCache cache,
    IOptions<LocalizeOptions> options,
    ILogger<DatabaseMissingKeyTracker> logger) : IMissingKeyTracker
{
    private readonly ConcurrentDictionary<string, byte> seen = new(StringComparer.Ordinal);

    public void Track(string scopeId, string key)
    {
        if (!options.Value.TrackMissingKeys || string.IsNullOrWhiteSpace(key) || key.Length > Constants.MaxLengths.TextKey)
        {
            return;
        }

        string token = scopeId + "\u001f" + key;
        if (!seen.TryAdd(token, 0))
        {
            return;
        }

        try
        {
            bool exists = strings.Exists(row =>
                row.ScopeId == scopeId && row.CultureCode == CultureCodes.Invariant && row.TextKey == key);

            if (exists)
            {
                return;
            }

            strings.Insert(new LocalizableString
            {
                ScopeId = scopeId,
                CultureCode = CultureCodes.Invariant,
                TextKey = key,
                TextValue = key,
            });
            cache.Invalidate();
        }
        catch (DbUpdateException)
        {
            // Another request stored this key first.
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            seen.TryRemove(token, out _);
            logger.LogDebug(ex, "Could not store missing localization key {Key}.", key);
        }
    }
}
