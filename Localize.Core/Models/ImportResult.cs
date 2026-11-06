namespace Localize.Models;

/// <summary>
/// Represents the import result.
/// </summary>
/// <param name="Added">The added.</param>
/// <param name="Updated">The updated.</param>
/// <param name="Skipped">The skipped.</param>
/// <param name="LanguageCreated">The language created.</param>
public sealed record ImportResult(int Added, int Updated, int Skipped, bool LanguageCreated);
