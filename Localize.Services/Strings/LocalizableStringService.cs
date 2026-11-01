namespace Localize.Strings;

public sealed class LocalizableStringService(
    IRepository<LocalizableString> strings,
    IRepository<Language> languages,
    ILocalizationCatalog catalog,
    ILocalizationScope scope,
    ILocalizationCache cache,
    IOptions<LocalizeOptions> options,
    IServiceScopeFactory scopes,
    ILogger<LocalizableStringService> logger) : ILocalizableStringService
{
    public async Task<Result<IReadOnlyList<ComparativeString>>> ListComparativeAsync(
        string? cultureCode,
        string? search,
        CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        var invariant = catalog.GetCulture(current, CultureCodes.Invariant);
        var localized = culture.Length == 0
            ? Empty
            : catalog.GetCulture(current, culture);

        var storedRows = await strings.FindAsync(new SearchOptions<LocalizableString>
        {
            Query = row => row.ScopeId == current && row.CultureCode == culture,
            CancellationToken = cancellationToken,
        });
        var storedKeys = storedRows.Select(row => row.TextKey).ToHashSet(StringComparer.Ordinal);

        var keys = new HashSet<string>(invariant.Keys, StringComparer.Ordinal);
        foreach (string key in localized.Keys)
        {
            keys.Add(key);
        }

        foreach (string key in storedKeys)
        {
            keys.Add(key);
        }

        IEnumerable<ComparativeString> items = keys
            .Select(key =>
            {
                invariant.TryGetValue(key, out string? invariantValue);
                string? localizedValue = localized.TryGetValue(key, out string? value) ? value : null;
                return new ComparativeString(key, invariantValue, localizedValue, storedKeys.Contains(key));
            })
            .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(search))
        {
            items = items.Where(item =>
                item.Key.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (item.InvariantValue?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.LocalizedValue?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return Result<IReadOnlyList<ComparativeString>>.Success(items.ToArray());
    }

    public async Task<Result> SaveAsync(
        string? cultureCode,
        string key,
        string? value,
        CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        key = key.Trim();

        if (key.Length == 0)
        {
            return Result.Invalid(new ValidationError(nameof(key), "A key is required."));
        }

        if (key.Length > Constants.MaxLengths.TextKey)
        {
            return Result.Invalid(new ValidationError(nameof(key), "That key is too long."));
        }

        var existing = await FindRowAsync(current, culture, key, cancellationToken);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (culture.Length == 0)
            {
                return Result.Invalid(new ValidationError(nameof(value), "The default text can't be empty. Delete the key instead."));
            }

            if (existing is not null)
            {
                await strings.DeleteAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
                cache.Invalidate();
            }

            return Result.Success();
        }

        if (existing is null)
        {
            await strings.InsertAsync(new LocalizableString
            {
                ScopeId = current,
                CultureCode = culture,
                TextKey = key,
                TextValue = value,
            }, ContextOptions.ForCancellationToken(cancellationToken));
        }
        else
        {
            existing.TextValue = value;
            await strings.UpdateAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
        }

        cache.Invalidate();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string? cultureCode, string key, CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        var existing = await FindRowAsync(current, culture, key.Trim(), cancellationToken);
        if (existing is null)
        {
            return Result.NotFound("That string is not stored.");
        }

        await strings.DeleteAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
        cache.Invalidate();
        return Result.Success();
    }

    public async Task<Result<LanguagePackDocument>> ExportAsync(string? cultureCode, CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        var page = await strings.FindAsync(new SearchOptions<LocalizableString>
        {
            Query = row => row.ScopeId == current && row.CultureCode == culture && row.TextValue != null,
            OrderBy = query => query.OrderBy(row => row.TextKey),
            CancellationToken = cancellationToken,
        });

        var document = new LanguagePackDocument
        {
            CultureCode = culture.Length == 0 ? null : culture,
        };

        foreach (var row in page)
        {
            document.LocalizedStrings[row.TextKey] = row.TextValue!;
        }

        return Result<LanguagePackDocument>.Success(document);
    }

    public async Task<Result<ImportResult>> ImportAsync(
        string json,
        string? cultureCode = null,
        CancellationToken cancellationToken = default)
    {
        if (!JsonLocalizationReader.TryRead(json, out string? documentCulture, out var incoming, out string? error))
        {
            return Result<ImportResult>.Invalid(new ValidationError("file", error ?? "That file could not be read."));
        }

        string culture = CultureCodes.Normalize(documentCulture ?? cultureCode);
        if (culture.Length > 0 && !CultureCodes.IsKnown(culture))
        {
            return Result<ImportResult>.Invalid(new ValidationError(nameof(cultureCode), "That is not a known culture code."));
        }

        string current = CurrentScope();
        HashSet<string>? known = null;
        if (options.Value.ImportOnlyKnownKeys)
        {
            var invariant = await strings.FindAsync(new SearchOptions<LocalizableString>
            {
                Query = row => row.ScopeId == current && row.CultureCode == CultureCodes.Invariant,
                CancellationToken = cancellationToken,
            });
            known = invariant.Select(row => row.TextKey).ToHashSet(StringComparer.Ordinal);
        }

        int added = 0;
        int updated = 0;
        int skipped = 0;
        foreach (var (rawKey, value) in incoming)
        {
            string key = rawKey.Trim();
            if (key.Length == 0 || key.Length > Constants.MaxLengths.TextKey || value is null)
            {
                skipped++;
                continue;
            }

            if (known is not null && !known.Contains(key))
            {
                skipped++;
                continue;
            }

            var existing = await FindRowAsync(current, culture, key, cancellationToken);
            if (existing is null)
            {
                await strings.InsertAsync(new LocalizableString
                {
                    ScopeId = current,
                    CultureCode = culture,
                    TextKey = key,
                    TextValue = value,
                }, ContextOptions.ForCancellationToken(cancellationToken));
                added++;
            }
            else if (existing.TextValue != value)
            {
                existing.TextValue = value;
                await strings.UpdateAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
                updated++;
            }
            else
            {
                skipped++;
            }
        }

        bool languageCreated = false;
        if (culture.Length > 0 && (added > 0 || updated > 0))
        {
            languageCreated = await EnsureLanguageAsync(current, culture, cancellationToken);
        }

        cache.Invalidate();
        logger.LogInformation(
            "Imported {Added} new and {Updated} updated strings for {Culture}. Skipped {Skipped}.",
            added,
            updated,
            culture.Length == 0 ? "invariant" : culture,
            skipped);

        return Result<ImportResult>.Success(new ImportResult(added, updated, skipped, languageCreated));
    }

    public async Task<Result<int>> ResetToPacksAsync(CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        var rows = ReadPacks(current);
        await strings.DeleteAsync(row => row.ScopeId == current, ContextOptions.ForCancellationToken(cancellationToken));
        if (rows.Count > 0)
        {
            await strings.InsertAsync(rows, ContextOptions.ForCancellationToken(cancellationToken));
        }

        cache.Invalidate();
        logger.LogInformation("Reset {Count} strings in scope {Scope} from language packs.", rows.Count, current);
        return Result<int>.Success(rows.Count);
    }

    private List<LocalizableString> ReadPacks(string current)
    {
        var rows = new Dictionary<(string Culture, string Key), string>();
        using var scope = scopes.CreateScope();
        foreach (var pack in scope.ServiceProvider.GetServices<ILanguagePack>())
        {
            if (pack.LocalizedStrings is null)
            {
                continue;
            }

            string culture = CultureCodes.Normalize(pack.CultureCode);
            foreach (var (rawKey, value) in pack.LocalizedStrings)
            {
                if (string.IsNullOrWhiteSpace(rawKey) || rawKey.Length > Constants.MaxLengths.TextKey || value is null)
                {
                    continue;
                }

                rows[(culture, rawKey.Trim())] = value;
            }
        }

        return rows.Select(pair => new LocalizableString
        {
            ScopeId = current,
            CultureCode = pair.Key.Culture,
            TextKey = pair.Key.Key,
            TextValue = pair.Value,
        }).ToList();
    }

    private async Task<bool> EnsureLanguageAsync(string current, string culture, CancellationToken cancellationToken)
    {
        bool exists = await languages.ExistsAsync(
            language => language.ScopeId == current && language.CultureCode == culture,
            ContextOptions.ForCancellationToken(cancellationToken));

        if (exists)
        {
            return false;
        }

        await languages.InsertAsync(new Language
        {
            ScopeId = current,
            CultureCode = culture,
            Name = CultureCodes.DisplayName(culture),
            IsRightToLeft = CultureCodes.IsRightToLeft(culture),
            IsEnabled = false,
            SortOrder = 0,
        }, ContextOptions.ForCancellationToken(cancellationToken));

        return true;
    }

    private async Task<LocalizableString?> FindRowAsync(
        string current,
        string culture,
        string key,
        CancellationToken cancellationToken) =>
        await strings.FindOneAsync(new SearchOptions<LocalizableString>
        {
            Query = row => row.ScopeId == current && row.CultureCode == culture && row.TextKey == key,
            CancellationToken = cancellationToken,
        });

    private string CurrentScope() => CultureCodes.NormalizeScope(scope.GetScopeId());

    private static readonly IReadOnlyDictionary<string, string> Empty = new Dictionary<string, string>();
}
