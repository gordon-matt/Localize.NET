namespace Localize.Languages;

/// <summary>
/// Defines the contract for the language service.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Lists the value.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the languages.</returns>
    Task<Result<IReadOnlyList<LanguageModel>>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the enabled.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the languages.</returns>
    Task<Result<IReadOnlyList<LanguageModel>>> ListEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the language.</returns>
    Task<Result<LanguageModel>> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the value.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the outcome for the language.</returns>
    Task<Result<LanguageModel>> SaveAsync(LanguageForm form, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the value.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result.</returns>
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Inserts the configured default culture for the current scope when it is missing.</summary>
    Task<Result<LanguageModel>> EnsureDefaultAsync(CancellationToken cancellationToken = default);
}
