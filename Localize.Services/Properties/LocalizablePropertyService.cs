namespace Localize.Properties;

/// <summary>
/// Represents the localizable property service.
/// </summary>
public sealed class LocalizablePropertyService(
    IRepository<LocalizableProperty> properties,
    ILocalizationScope scope) : ILocalizablePropertyService
{
    /// <summary>
    /// Finds the one.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="property">The property.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the localizable property.</returns>
    public async Task<Result<LocalizablePropertyModel>> FindOneAsync(
        string? cultureCode,
        string entityType,
        string entityId,
        string property,
        CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        var entity = await properties.FindOneAsync(new SearchOptions<LocalizableProperty>
        {
            Query = row => row.ScopeId == current
                && row.CultureCode == culture
                && row.EntityType == entityType
                && row.EntityId == entityId
                && row.Property == property,
            CancellationToken = cancellationToken,
        });

        return entity is null
            ? Result<LocalizablePropertyModel>.NotFound()
            : Result<LocalizablePropertyModel>.Success(ToModel(entity));
    }

    /// <summary>
    /// Finds the value.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityIds">The entity ids.</param>
    /// <param name="property">The property.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the localizable properties.</returns>
    public async Task<Result<IReadOnlyList<LocalizablePropertyModel>>> FindAsync(
        string? cultureCode,
        string entityType,
        IEnumerable<string> entityIds,
        string? property = null,
        CancellationToken cancellationToken = default)
    {
        string[] ids = entityIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Result<IReadOnlyList<LocalizablePropertyModel>>.Success([]);
        }

        string current = CurrentScope();
        string culture = CultureCodes.Normalize(cultureCode);
        var page = await properties.FindAsync(new SearchOptions<LocalizableProperty>
        {
            Query = row => row.ScopeId == current
                && row.CultureCode == culture
                && row.EntityType == entityType
                && ids.Contains(row.EntityId)
                && (property == null || row.Property == property),
            CancellationToken = cancellationToken,
        });

        return Result<IReadOnlyList<LocalizablePropertyModel>>.Success(page.Select(ToModel).ToArray());
    }

    /// <summary>
    /// Saves the value.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the localizable property.</returns>
    public async Task<Result<LocalizablePropertyModel>> SaveAsync(
        LocalizablePropertyForm form,
        CancellationToken cancellationToken = default)
    {
        if (CultureCodes.IsInvariant(form.CultureCode))
        {
            return Result<LocalizablePropertyModel>.Invalid(new ValidationError(
                nameof(form.CultureCode),
                "Property translations need a culture. The default text stays on the entity."));
        }

        if (!CultureCodes.IsKnown(form.CultureCode))
        {
            return Result<LocalizablePropertyModel>.Invalid(new ValidationError(
                nameof(form.CultureCode),
                "That is not a known culture code."));
        }

        if (string.IsNullOrWhiteSpace(form.EntityType) || form.EntityType.Length > Constants.MaxLengths.EntityType)
        {
            return Result<LocalizablePropertyModel>.Invalid(new ValidationError(nameof(form.EntityType), "Entity type is required."));
        }

        if (string.IsNullOrWhiteSpace(form.EntityId) || form.EntityId.Length > Constants.MaxLengths.EntityId)
        {
            return Result<LocalizablePropertyModel>.Invalid(new ValidationError(nameof(form.EntityId), "Entity id is required."));
        }

        if (string.IsNullOrWhiteSpace(form.Property) || form.Property.Length > Constants.MaxLengths.Property)
        {
            return Result<LocalizablePropertyModel>.Invalid(new ValidationError(nameof(form.Property), "Property name is required."));
        }

        string current = CurrentScope();
        string culture = CultureCodes.Normalize(form.CultureCode);
        var existing = await properties.FindOneAsync(new SearchOptions<LocalizableProperty>
        {
            Query = row => row.ScopeId == current
                && row.CultureCode == culture
                && row.EntityType == form.EntityType
                && row.EntityId == form.EntityId
                && row.Property == form.Property,
            CancellationToken = cancellationToken,
        });

        if (string.IsNullOrEmpty(form.Value))
        {
            if (existing is not null)
            {
                await properties.DeleteAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
            }

            return Result<LocalizablePropertyModel>.Success(new LocalizablePropertyModel(
                existing?.Id ?? 0,
                culture,
                form.EntityType,
                form.EntityId,
                form.Property,
                null));
        }

        if (existing is null)
        {
            var inserted = await properties.InsertAsync(new LocalizableProperty
            {
                ScopeId = current,
                CultureCode = culture,
                EntityType = form.EntityType,
                EntityId = form.EntityId,
                Property = form.Property,
                Value = form.Value,
            }, ContextOptions.ForCancellationToken(cancellationToken));

            return Result<LocalizablePropertyModel>.Success(ToModel(inserted));
        }

        existing.Value = form.Value;
        await properties.UpdateAsync(existing, ContextOptions.ForCancellationToken(cancellationToken));
        return Result<LocalizablePropertyModel>.Success(ToModel(existing));
    }

    /// <summary>
    /// Deletes for the entity.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result.</returns>
    public async Task<Result> DeleteForEntityAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default)
    {
        string current = CurrentScope();
        await properties.DeleteAsync(
            row => row.ScopeId == current && row.EntityType == entityType && row.EntityId == entityId,
            ContextOptions.ForCancellationToken(cancellationToken));

        return Result.Success();
    }

    private string CurrentScope() => CultureCodes.NormalizeScope(scope.GetScopeId());

    private static LocalizablePropertyModel ToModel(LocalizableProperty row) => new(
        row.Id,
        row.CultureCode,
        row.EntityType,
        row.EntityId,
        row.Property,
        row.Value);
}
