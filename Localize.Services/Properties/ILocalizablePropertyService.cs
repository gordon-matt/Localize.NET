namespace Localize.Properties;

/// <summary>
/// Defines the contract for the localizable property service.
/// </summary>
public interface ILocalizablePropertyService
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
    Task<Result<LocalizablePropertyModel>> FindOneAsync(
        string? cultureCode,
        string entityType,
        string entityId,
        string property,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the value.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityIds">The entity ids.</param>
    /// <param name="property">The property.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the localizable properties.</returns>
    Task<Result<IReadOnlyList<LocalizablePropertyModel>>> FindAsync(
        string? cultureCode,
        string entityType,
        IEnumerable<string> entityIds,
        string? property = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the value.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the localizable property.</returns>
    Task<Result<LocalizablePropertyModel>> SaveAsync(
        LocalizablePropertyForm form,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes for the entity.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result.</returns>
    Task<Result> DeleteForEntityAsync(
        string entityType,
        string entityId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides extension methods for localizable property services.
/// </summary>
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
