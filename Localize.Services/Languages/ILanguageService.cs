namespace Localize.Languages;

public interface ILanguageService
{
    Task<Result<IReadOnlyList<LanguageModel>>> ListAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<LanguageModel>>> ListEnabledAsync(CancellationToken cancellationToken = default);

    Task<Result<LanguageModel>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<LanguageModel>> SaveAsync(LanguageForm form, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Inserts the configured default culture for the current scope when it is missing.</summary>
    Task<Result<LanguageModel>> EnsureDefaultAsync(CancellationToken cancellationToken = default);
}
