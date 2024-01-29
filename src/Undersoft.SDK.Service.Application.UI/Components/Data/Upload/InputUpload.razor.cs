using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class InputUpload<TValue>
{
    private string? InputValueClassString => CssBuilder.Default("form-control")
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? RemoveButtonClassString => CssBuilder.Default()
        .AddClass(DeleteButtonClass)
        .Build();

    private bool IsDeleteButtonDisabled => IsDisabled || CurrentFile == null;

    private string? BrowserButtonClassString => CssBuilder.Default("btn-browser")
        .AddClass(BrowserButtonClass)
        .Build();

    private string? GetFileName() => CurrentFile?.GetFileName() ?? Value?.ToString();

    [Parameter]
    public string? BrowserButtonIcon { get; set; }

    [Parameter]
    public string BrowserButtonClass { get; set; } = "btn-primary";

    [Parameter]
    [NotNull]
    public string? BrowserButtonText { get; set; }

    [Parameter]
    public string DeleteButtonClass { get; set; } = "btn-danger";

    [Parameter]
    public string? DeleteButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonText { get; set; }

    [Parameter]
    public bool ShowDeleteButton { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<UploadBase<TValue>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DeleteButtonText ??= Localizer[nameof(DeleteButtonText)];
        BrowserButtonText ??= Localizer[nameof(BrowserButtonText)];
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        BrowserButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.InputUploadBrowserButtonIcon);
        DeleteButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.InputUploadDeleteButtonIcon);
    }

    protected override async Task OnFileChange(InputFileChangeEventArgs args)
    {
        CurrentFile = new UploadFile()
        {
            OriginFileName = args.File.Name,
            Size = args.File.Size,
            File = args.File,
            Uploaded = false
        };

        UploadFiles.Clear();
        UploadFiles.Add(CurrentFile);

        await base.OnFileChange(args);

        if (OnChange != null)
        {
            await OnChange(CurrentFile);
        }
        CurrentFile.Uploaded = true;
    }

    private async Task OnDeleteFile()
    {
        if (CurrentFile != null)
        {
            var ret = await OnFileDelete(CurrentFile);
            if (ret)
            {
                CurrentFile = null;
                CurrentValue = default;
            }
        }
    }

    public override void ToggleMessage(IEnumerable<ValidationResult> results, bool validProperty)
    {
        if (results.Any())
        {
            ErrorMessage = results.First().ErrorMessage;
            IsValid = false;
        }
        else
        {
            ErrorMessage = null;
            IsValid = true;
        }
        OnValidate(IsValid);
    }
}
