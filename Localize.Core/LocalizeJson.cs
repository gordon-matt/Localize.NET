namespace Localize;

/// <summary>
/// Provides localize JSON.
/// </summary>
public static class LocalizeJson
{
    /// <summary>
    /// Gets the options.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };
}
