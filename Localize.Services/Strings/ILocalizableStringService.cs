namespace Localize.Strings;

public interface ILocalizableStringService
{
    Task<Result<IReadOnlyList<ComparativeString>>> ListComparativeAsync(
        string? cultureCode,
        string? search,
        CancellationToken cancellationToken = default);

    Task<Result> SaveAsync(
        string? cultureCode,
        string key,
        string? value,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        string? cultureCode,
        string key,
        CancellationToken cancellationToken = default);

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
