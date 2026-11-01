namespace Localize.Properties;

public interface ILocalizablePropertyService
{
    Task<Result<LocalizablePropertyModel>> FindOneAsync(
        string? cultureCode,
        string entityType,
        string entityId,
        string property,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<LocalizablePropertyModel>>> FindAsync(
        string? cultureCode,
        string entityType,
        IEnumerable<string> entityIds,
        string? property = null,
        CancellationToken cancellationToken = default);

    Task<Result<LocalizablePropertyModel>> SaveAsync(
        LocalizablePropertyForm form,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteForEntityAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default);
}

public static class LocalizablePropertyServiceExtensions
{
    /// <summary>
    /// The stored translation, or <paramref name="fallback"/> when this culture has no row.
    /// An empty culture returns <paramref name="fallback"/> without a query, because the entity holds that text.
    /// </summary>
    public static async Task<string?> ValueOrDefaultAsync(
        this ILocalizablePropertyService service,
        string? cultureCode,
        string entityType,
        string entityId,
        string property,
        string? fallback,
        CancellationToken cancellationToken = default)
    {
        if (CultureCodes.IsInvariant(cultureCode))
        {
            return fallback;
        }

        var found = await service.FindOneAsync(cultureCode, entityType, entityId, property, cancellationToken);
        return found.IsSuccess ? found.Value.Value ?? fallback : fallback;
    }
}
