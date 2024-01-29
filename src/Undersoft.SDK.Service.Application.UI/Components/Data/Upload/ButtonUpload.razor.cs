using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class ButtonUpload<TValue>
{
    private bool IsUploadButtonDisabled => IsDisabled || (IsSingle && UploadFiles.Any());

    private string? BrowserButtonClassString => CssBuilder.Default("btn-browser")
        .AddClass(BrowserButtonClass, !string.IsNullOrEmpty(BrowserButtonClass))
        .Build();

    private string? LoadingIconString => CssBuilder.Default("loading-icon")
        .AddClass(LoadingIcon)
        .Build();

    private string? DeleteIconString => CssBuilder.Default("delete-icon")
        .AddClass(DeleteIcon)
        .Build();

    private string? ValidStatusIconString => CssBuilder.Default("valid-icon")
        .AddClass(ValidStatusIcon)
        .Build();

    private string? InvalidStatusIconString => CssBuilder.Default("invalid-icon")
        .AddClass(InvalidStatusIcon)
        .Build();

    private string? DownloadIconString => CssBuilder.Default("download-icon")
        .AddClass(DownloadIcon)
        .Build();

    [Parameter]
    public string? LoadingIcon { get; set; }

    [Parameter]
    public string? DownloadIcon { get; set; }

    [Parameter]
    public string? InvalidStatusIcon { get; set; }

    [Parameter]
    public string? ValidStatusIcon { get; set; }

    [Parameter]
    public string? DeleteIcon { get; set; }

    [Parameter]
    public string? BrowserButtonIcon { get; set; }

    [Parameter]
    public string? BrowserButtonClass { get; set; }

    [Parameter]
    public bool ShowUploadFileList { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? BrowserButtonText { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<UploadBase<TValue>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        BrowserButtonText ??= Localizer[nameof(BrowserButtonText)];
        BrowserButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadBrowserButtonIcon);
        LoadingIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadLoadingIcon);
        InvalidStatusIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadInvalidStatusIcon);
        ValidStatusIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadValidStatusIcon);
        DownloadIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadDownloadIcon);
        DeleteIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonUploadDeleteIcon);
    }
}
