namespace Localize;

/// <summary>
/// Defines the contract for the localization cache.
/// </summary>
public interface ILocalizationCache
{
    /// <summary>
    /// Gets the version.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Invalidates the value.
    /// </summary>
    void Invalidate();
}

/// <summary>
/// Represents the localization cache.
/// </summary>
public sealed class LocalizationCache : ILocalizationCache
{
    private int version = 1;

    /// <summary>
    /// Gets the version.
    /// </summary>
    public int Version => Volatile.Read(ref version);

    /// <summary>
    /// Invalidates the value.
    /// </summary>
    public void Invalidate() => Interlocked.Increment(ref version);
}
