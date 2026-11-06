namespace Localize.Strings;

/// <summary>
/// Defines the contract for the localizable string service.
/// </summary>
public interface ILocalizableStringService
{
    /// <summary>
    /// Lists the comparative.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="search">The search.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the comparative strings.</returns>
    Task<Result<IReadOnlyList<ComparativeString>>> ListComparativeAsync(
        string? cultureCode,
        string? search,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the value.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result.</returns>
    Task<Result> SaveAsync(
        string? cultureCode,
        string key,
        string? value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the value.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="key">The key.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result.</returns>
    Task<Result> DeleteAsync(
        string? cultureCode,
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports the value.
    /// </summary>
    /// <param name="cultureCode">The culture code.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the language pack document.</returns>
    Task<Result<LanguagePackDocument>> ExportAsync(
        string? cultureCode,
        CancellationToken cancellationToken = default);

    /// <param name="cultureCode">Used when the file does not name a culture. Null imports the file's culture, or invariant.</param>
    Task<Result<ImportResult>> ImportAsync(
        string json,
        string? cultureCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes every string in the current scope and inserts the registered language packs again.</summary>
    Task<Result<int>> ResetToPacksAsync(CancellationToken cancellationToken = default);
}
