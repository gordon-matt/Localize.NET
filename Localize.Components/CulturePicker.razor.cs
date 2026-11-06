namespace Localize.Components;

/// <summary>
/// Chooses a culture from the ones .NET knows: a language first, then optionally one of its regions.
/// The value is the culture code, or empty when nothing is chosen.
/// </summary>
public partial class CulturePicker
{
    private string language = "";
    private IReadOnlyList<CultureChoice> regions = [];
    private bool isUnlisted;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the value changed.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>Id of the language dropdown, so a label can point at it.</summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Shown when no language is chosen.</summary>
    [Parameter]
    public string Placeholder { get; set; } = "Choose a language";

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        string? root = CultureCodes.RootNeutral(Value);
        regions = CultureCodes.SpecificCulturesOf(root);

        // A stored code .NET no longer lists would otherwise vanish from the form and be lost on save.
        isUnlisted = !string.IsNullOrWhiteSpace(Value) && root is null;
        language = root ?? (isUnlisted ? Value! : "");
    }

    private Task LanguageChangedAsync(ChangeEventArgs args) => SetAsync(args.Value?.ToString());

    private Task RegionChangedAsync(ChangeEventArgs args) => SetAsync(args.Value?.ToString());

    private async Task SetAsync(string? code)
    {
        Value = code ?? "";
        await ValueChanged.InvokeAsync(Value);
    }
}