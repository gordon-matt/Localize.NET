namespace Localize;

public interface ILocalizationCache
{
    int Version { get; }

    void Invalidate();
}

public sealed class LocalizationCache : ILocalizationCache
{
    private int version = 1;

    public int Version => Volatile.Read(ref version);

    public void Invalidate() => Interlocked.Increment(ref version);
}
