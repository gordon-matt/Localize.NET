namespace Localize;

/// <summary>Help text read from the string catalog. Read <see cref="HelpText"/> from the attribute.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class LocalizedHelpTextAttribute(string key) : Attribute
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets the help text.
    /// </summary>
    public string HelpText => LocalizeRuntime.GetString(Key);
}
