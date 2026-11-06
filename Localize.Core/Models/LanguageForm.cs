namespace Localize.Models;

/// <summary>
/// Represents the language form.
/// </summary>
public sealed class LanguageForm
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether this instance is right to left.
    /// </summary>
    public bool IsRightToLeft { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public int SortOrder { get; set; }
}
