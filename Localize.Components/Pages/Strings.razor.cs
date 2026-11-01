namespace Localize.Components.Pages;

public partial class Strings
{
    private static readonly int[] PageSizes = [25, 50, 100];

    [Parameter]
    public string? CultureCode { get; set; }

    [Inject]
    public ILocalizableStringService StringService { get; set; } = default!;

    [Inject]
    public IOptions<LocalizeOptions> Options { get; set; } = default!;

    [Inject]
    public IServiceProvider Services { get; set; } = default!;

    [Inject]
    public ToastService Toasts { get; set; } = default!;

    [Inject]
    public DialogService Dialogs { get; set; } = default!;

    private IReadOnlyList<ComparativeString> rows = [];
    private ComparativeString? selected;
    private string? draft;
    private string? search;
    private string? loadError;
    private string? error;
    private string? loadedFor;
    private bool loading = true;
    private bool denied;
    private bool accessChecked;
    private bool busy;
    private int pageNumber = 1;
    private int pageSize = 25;

    private bool IsInvariant => CultureCodes.IsInvariant(CultureCode);

    private string CultureLabel => IsInvariant ? "Default" : CultureCodes.DisplayName(CultureCodes.Normalize(CultureCode));

    private string Title => IsInvariant ? "Default strings" : $"{CultureLabel} strings";

    private string Description => IsInvariant
        ? "This text is used when a culture has no translation of its own."
        : "A blank translation falls back to the default, then to a parent culture.";

    private string ExportHref => $"{LocalizeRoutes.Export}/{(IsInvariant ? "invariant" : CultureCodes.Normalize(CultureCode))}";

    private IReadOnlyList<ComparativeString> filtered =>
        string.IsNullOrWhiteSpace(search)
            ? rows
            : rows.Where(row =>
                row.Key.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (row.InvariantValue?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                || (row.LocalizedValue?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToArray();

    private IEnumerable<ComparativeString> pageRows =>
        filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize);

    protected override async Task OnParametersSetAsync()
    {
        if (!accessChecked)
        {
            denied = !await LocalizeAccess.AllowAsync(Options.Value, Services);
            accessChecked = true;
        }

        if (denied)
        {
            loading = false;
            return;
        }

        string key = CultureCode ?? "";
        if (loadedFor == key)
        {
            return;
        }

        loadedFor = key;
        pageNumber = 1;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        loading = true;
        var listed = await StringService.ListComparativeAsync(CultureCode, search: null);
        loading = false;
        if (!listed.IsSuccess)
        {
            loadError = listed.GetDisplayMessage();
            rows = [];
            return;
        }

        loadError = null;
        rows = listed.Value;
    }

    private void Edit(ComparativeString row)
    {
        selected = row;
        draft = IsInvariant ? row.InvariantValue : row.LocalizedValue;
        error = null;
    }

    private void CloseEditor() => selected = null;

    private async Task SaveAsync()
    {
        if (selected is null)
        {
            return;
        }

        busy = true;
        var saved = await StringService.SaveAsync(CultureCode, selected.Key, draft);
        busy = false;
        if (!saved.IsSuccess)
        {
            error = saved.GetDisplayMessage();
            return;
        }

        selected = null;
        Toasts.Success("String saved.");
        loadedFor = null;
        await OnParametersSetAsync();
    }

    private async Task DeleteAsync()
    {
        if (selected is null)
        {
            return;
        }

        bool confirmed = await Dialogs.ConfirmAsync(
            "Delete this string?",
            IsInvariant
                ? "The key is removed for the default culture."
                : "This culture will fall back to the default text.",
            "Delete",
            isDestructive: true);

        if (!confirmed)
        {
            return;
        }

        busy = true;
        var deleted = await StringService.DeleteAsync(CultureCode, selected.Key);
        busy = false;
        if (!deleted.IsSuccess)
        {
            error = deleted.GetDisplayMessage();
            return;
        }

        selected = null;
        Toasts.Success("String deleted.");
        loadedFor = null;
        await OnParametersSetAsync();
    }

    private Task OnSearch(string? value)
    {
        search = value;
        pageNumber = 1;
        return Task.CompletedTask;
    }

    private Task ChangePage(int number)
    {
        pageNumber = number;
        return Task.CompletedTask;
    }

    private Task ChangePageSize(int size)
    {
        pageSize = size;
        pageNumber = 1;
        return Task.CompletedTask;
    }
}
