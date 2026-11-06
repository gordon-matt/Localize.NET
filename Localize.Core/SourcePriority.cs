namespace Localize;

/// <summary>Default priorities. A higher number wins when two sources have the same key.</summary>
public static class SourcePriority
{
    /// <summary>
    /// The language pack constant.
    /// </summary>
    public const int LanguagePack = 10;

    /// <summary>
    /// The resx constant.
    /// </summary>
    public const int Resx = 40;

    /// <summary>
    /// The JSON constant.
    /// </summary>
    public const int Json = 50;

    /// <summary>
    /// The database constant.
    /// </summary>
    public const int Database = 100;

    /// <summary>Used when <see cref="LocalizeOptions.JsonOverridesDatabase"/> is set.</summary>
    public const int JsonOverride = 200;
}
