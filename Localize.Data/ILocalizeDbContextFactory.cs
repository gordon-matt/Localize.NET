namespace Localize.Data;

/// <summary>
/// Localize's own context factory. It extends Extenso's <see cref="IDbContextFactory"/> instead of
/// replacing it, so a host that already registered that interface for its own database is left alone.
/// </summary>
public interface ILocalizeDbContextFactory : IDbContextFactory
{
    /// <summary>One of <see cref="Constants.DatabaseProviders"/>.</summary>
    string ProviderName { get; }
}
