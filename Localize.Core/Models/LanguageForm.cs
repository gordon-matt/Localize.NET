namespace Localize.Models;

public sealed class LanguageForm
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string CultureCode { get; set; } = "";

    public bool IsRightToLeft { get; set; }

    public bool IsEnabled { get; set; } = true;

    public int SortOrder { get; set; }
}
