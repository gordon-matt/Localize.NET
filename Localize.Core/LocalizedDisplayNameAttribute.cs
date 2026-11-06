using System.ComponentModel;

namespace Localize;

/// <summary>Display name read from the string catalog. <paramref name="key"/> is the catalog key.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class LocalizedDisplayNameAttribute(string key) : DisplayNameAttribute(key)
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; } = key;

    /// <inheritdoc/>
    public override string DisplayName => LocalizeRuntime.GetString(Key);
}
