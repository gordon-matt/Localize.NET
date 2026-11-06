namespace Localize.Models;

/// <summary>One key shown beside the invariant text and the selected culture.</summary>
public sealed record ComparativeString(string Key, string? InvariantValue, string? LocalizedValue, bool IsStored)
{
    /// <summary>
    /// Gets a value indicating whether this instance has translation.
    /// </summary>
    public bool HasTranslation => LocalizedValue is not null;
}
