using System.ComponentModel;

namespace Localize;

/// <summary>Display name read from the string catalog. <paramref name="key"/> is the catalog key.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class LocalizedDisplayNameAttribute(string key) : DisplayNameAttribute(key)
{
    public string Key { get; } = key;

    public override string DisplayName => LocalizeRuntime.GetString(Key);
}
