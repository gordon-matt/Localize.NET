namespace Localize.Languages;

public sealed class LanguageService(
    IRepository<Language> languages,
    IRepository<LocalizableString> strings,
    ILocalizationScope scope,
    ILocalizationCache cache,
    IOptions<LocalizeOptions> options,
    ILogger<LanguageService> logger) : ILanguageService
{
    public async Task<Result<IReadOnlyList<LanguageModel>>> ListAsync(CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        var page = await languages.FindAsync(new SearchOptions<Language>
        {
            Query = language => language.ScopeId == current,
            OrderBy = query => query.OrderBy(language => language.SortOrder).ThenBy(language => language.Name),
            CancellationToken = cancellationToken,
        });

        return Result<IReadOnlyList<LanguageModel>>.Success(page.Select(ToModel).ToArray());
    }

    public async Task<Result<IReadOnlyList<LanguageModel>>> ListEnabledAsync(CancellationToken cancellationToken = default)
    {
        var listed = await ListAsync(cancellationToken);
        if (!listed.IsSuccess)
        {
            return listed;
        }

        return Result<IReadOnlyList<LanguageModel>>.Success(listed.Value.Where(language => language.IsEnabled).ToArray());
    }

    public async Task<Result<LanguageModel>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var found = await FindInScopeAsync(id, cancellationToken);
        return found.IsSuccess ? Result<LanguageModel>.Success(ToModel(found.Value)) : found.To<LanguageModel>();
    }

    public async Task<Result<LanguageModel>> SaveAsync(LanguageForm form, CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        var validation = Validate(form, creating: form.Id == 0);
        if (!validation.IsSuccess)
        {
            return validation.To<LanguageModel>();
        }

        string culture = CultureCodes.Normalize(form.CultureCode);
        var duplicate = await languages.FindOneAsync(new SearchOptions<Language>
        {
            Query = language => language.ScopeId == current && language.CultureCode == culture && language.Id != form.Id,
            CancellationToken = cancellationToken,
        });

        if (duplicate is not null)
        {
            return Result<LanguageModel>.Invalid(new ValidationError(
                nameof(form.CultureCode),
                $"{CultureCodes.DisplayName(culture)} is already in the list."));
        }

        if (form.Id == 0)
        {
            var inserted = await languages.InsertAsync(new Language
            {
                ScopeId = current,
                Name = form.Name.Trim(),
                CultureCode = culture,
                IsRightToLeft = form.IsRightToLeft,
                IsEnabled = form.IsEnabled,
                SortOrder = form.SortOrder,
            }, ContextOptions.ForCancellationToken(cancellationToken));

            logger.LogInformation("Added language {Culture} in scope {Scope}.", culture, current);
            return Result<LanguageModel>.Success(ToModel(inserted));
        }

        var existing = await FindInScopeAsync(form.Id, cancellationToken);
        if (!existing.IsSuccess)
        {
            return existing.To<LanguageModel>();
        }

        var entity = existing.Value;
        entity.Name = form.Name.Trim();
        entity.IsRightToLeft = form.IsRightToLeft;
        entity.IsEnabled = form.IsEnabled;
        entity.SortOrder = form.SortOrder;
        await languages.UpdateAsync(entity, ContextOptions.ForCancellationToken(cancellationToken));
        return Result<LanguageModel>.Success(ToModel(entity));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await FindInScopeAsync(id, cancellationToken);
        if (!existing.IsSuccess)
        {
            return existing.ToVoid();
        }

        string culture = existing.Value.CultureCode;
        string current = existing.Value.ScopeId;
        await languages.DeleteAsync(existing.Value, ContextOptions.ForCancellationToken(cancellationToken));
        await strings.DeleteAsync(
            row => row.ScopeId == current && row.CultureCode == culture,
            ContextOptions.ForCancellationToken(cancellationToken));
        cache.Invalidate();
        logger.LogInformation("Removed language {Culture} in scope {Scope}.", culture, current);
        return Result.Success();
    }

    public async Task<Result<LanguageModel>> EnsureDefaultAsync(CancellationToken cancellationToken = default)
    {
        if (!CultureCodes.IsKnown(options.Value.DefaultCulture))
        {
            return Result<LanguageModel>.Invalid(new ValidationError(
                nameof(LocalizeOptions.DefaultCulture),
                "The default culture is not a known culture."));
        }

        string culture = CultureCodes.Normalize(options.Value.DefaultCulture);
        string current = CurrentScope();
        var existing = await languages.FindOneAsync(new SearchOptions<Language>
        {
            Query = language => language.ScopeId == current && language.CultureCode == culture,
            CancellationToken = cancellationToken,
        });

        if (existing is not null)
        {
            return Result<LanguageModel>.Success(ToModel(existing));
        }

        return await SaveAsync(new LanguageForm
        {
            Name = CultureCodes.DisplayName(culture),
            CultureCode = culture,
            IsRightToLeft = CultureCodes.IsRightToLeft(culture),
            IsEnabled = true,
        }, cancellationToken);
    }

    private async Task<Result<Language>> FindInScopeAsync(int id, CancellationToken cancellationToken)
    {
        string current = CurrentScope();
        var entity = await languages.FindOneAsync(new SearchOptions<Language>
        {
            Query = language => language.Id == id && language.ScopeId == current,
            CancellationToken = cancellationToken,
        });

        return entity is null
            ? Result<Language>.NotFound("That language is not in the list.")
            : Result<Language>.Success(entity);
    }

    private string CurrentScope() => CultureCodes.NormalizeScope(scope.GetScopeId());

    private static Result Validate(LanguageForm form, bool creating)
    {
        if (creating && string.IsNullOrWhiteSpace(form.CultureCode))
        {
            return Result.Invalid(new ValidationError(nameof(form.CultureCode), "Choose a culture."));
        }

        if (creating && !CultureCodes.IsKnown(form.CultureCode))
        {
            return Result.Invalid(new ValidationError(nameof(form.CultureCode), "That is not a known culture code."));
        }

        if (creating && CultureCodes.Normalize(form.CultureCode).Length > Constants.MaxLengths.CultureCode)
        {
            return Result.Invalid(new ValidationError(nameof(form.CultureCode), "That culture code is too long."));
        }

        if (string.IsNullOrWhiteSpace(form.Name))
        {
            return Result.Invalid(new ValidationError(nameof(form.Name), "Give the language a name."));
        }

        if (form.Name.Trim().Length > Constants.MaxLengths.Name)
        {
            return Result.Invalid(new ValidationError(nameof(form.Name), "That name is too long."));
        }

        return Result.Success();
    }

    private static LanguageModel ToModel(Language language) => new(
        language.Id,
        language.Name,
        language.CultureCode,
        language.IsRightToLeft,
        language.IsEnabled,
        language.SortOrder);
}
