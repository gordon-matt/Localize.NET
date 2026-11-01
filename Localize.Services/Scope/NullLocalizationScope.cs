namespace Localize.Scope;

internal sealed class NullLocalizationScope : ILocalizationScope
{
    public string? GetScopeId() => null;
}
