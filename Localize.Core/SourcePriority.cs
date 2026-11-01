namespace Localize;

/// <summary>Default priorities. A higher number wins when two sources have the same key.</summary>
public static class SourcePriority
{
    public const int LanguagePack = 10;

    public const int Resx = 40;

    public const int Json = 50;

    public const int Database = 100;

    /// <summary>Used when <see cref="LocalizeOptions.JsonOverridesDatabase"/> is set.</summary>
    public const int JsonOverride = 200;
}
