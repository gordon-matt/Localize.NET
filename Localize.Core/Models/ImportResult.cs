namespace Localize.Models;

public sealed record ImportResult(int Added, int Updated, int Skipped, bool LanguageCreated);
