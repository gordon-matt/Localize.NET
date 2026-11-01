namespace Localize.Tracking;

internal interface IMissingKeyTracker
{
    void Track(string scopeId, string key);
}

internal sealed class NullMissingKeyTracker : IMissingKeyTracker
{
    public void Track(string scopeId, string key)
    {
    }
}
