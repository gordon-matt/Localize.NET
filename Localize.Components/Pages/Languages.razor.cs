using Microsoft.AspNetCore.Components.Forms;

namespace Localize.Components.Pages;

public partial class Languages
{
    [Inject]
    public ILanguageService LanguageService { get; set; } = default!;

    [Inject]
    public ILocalizableStringService Strings { get; set; } = default!;

    [Inject]
    public IOptions<LocalizeOptions> Options { get; set; } = default!;

    [Inject]
    public IServiceProvider Services { get; set; } = default!;

    [Inject]
    public ToastService Toasts { get; set; } = default!;

    [Inject]
    public DialogService Dialogs { get; set; } = default!;

    private List<LanguageModel> languages = [];
    private LanguageForm form = new();
    private string? importCulture;
    private string? loadError;
    private string? error;
    private bool loading = true;
    private bool denied;
    private bool editing;
    private bool busy;

    protected override async Task OnInitializedAsync()
    {
        if (!await LocalizeAccess.AllowAsync(Options.Value, Services))
        {
            denied = true;
            loading = false;
            return;
        }

        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        loading = true;
        var listed = await LanguageService.ListAsync();
        loading = false;
        if (!listed.IsSuccess)
        {
            loadError = listed.GetDisplayMessage();
            return;
        }

        loadError = null;
        languages = [.. listed.Value];
    }

    private void NewLanguage()
    {
        form = new LanguageForm();
        error = null;
        editing = true;
    }

    private void Edit(LanguageModel language)
    {
        form = new LanguageForm
        {
            Id = language.Id,
            Name = language.Name,
            CultureCode = language.CultureCode,
            IsRightToLeft = language.IsRightToLeft,
            IsEnabled = language.IsEnabled,
            SortOrder = language.SortOrder,
        };
        error = null;
        editing = true;
    }

    private void CloseEditor() => editing = false;

    private async Task SaveAsync()
    {
        if (form.Id == 0 && string.IsNullOrWhiteSpace(form.Name) && CultureCodes.IsKnown(form.CultureCode))
        {
            form.Name = CultureCodes.DisplayName(form.CultureCode);
            form.IsRightToLeft = CultureCodes.IsRightToLeft(form.CultureCode);
        }

        busy = true;
        var saved = await LanguageService.SaveAsync(form);
        busy = false;
        if (!saved.IsSuccess)
        {
            error = saved.GetDisplayMessage();
            return;
        }

        editing = false;
        Toasts.Success(form.Id == 0 ? "Language added." : "Language saved.");
        await LoadAsync();
    }

    private async Task DeleteAsync(LanguageModel language)
    {
        bool confirmed = await Dialogs.ConfirmAsync(
            $"Delete {language.Name}?",
            "Its translations are removed. The default text stays.",
            "Delete",
            isDestructive: true);

        if (!confirmed)
        {
            return;
        }

        busy = true;
        var deleted = await LanguageService.DeleteAsync(language.Id);
        busy = false;
        if (!deleted.IsSuccess)
        {
            Toasts.ShowFailure(deleted);
            return;
        }

        Toasts.Success($"{language.Name} was deleted.");
        await LoadAsync();
    }

    private async Task ResetAsync()
    {
        bool confirmed = await Dialogs.ConfirmAsync(
            "Reset strings?",
            "Every stored string in this scope is replaced with the language packs shipped in code. Editor changes are lost. File and RESX text is left alone.",
            "Reset",
            isDestructive: true);

        if (!confirmed)
        {
            return;
        }

        busy = true;
        var reset = await Strings.ResetToPacksAsync();
        busy = false;
        if (!reset.IsSuccess)
        {
            Toasts.ShowFailure(reset);
            return;
        }

        Toasts.Success($"Restored {reset.Value} strings from language packs.");
    }

    private async Task ImportAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file.Size > 2 * 1024 * 1024)
        {
            Toasts.Error("That file is larger than 2 MB.");
            return;
        }

        busy = true;
        try
        {
            await using var stream = file.OpenReadStream(2 * 1024 * 1024);
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            var imported = await Strings.ImportAsync(json, importCulture);
            if (!imported.IsSuccess)
            {
                Toasts.ShowFailure(imported);
                return;
            }

            var result = imported.Value;
            string created = result.LanguageCreated ? " A new language was added, switched off." : "";
            Toasts.Success($"Imported {result.Added} new and {result.Updated} updated.{created}");
            await LoadAsync();
        }
        finally
        {
            busy = false;
        }
    }
}
