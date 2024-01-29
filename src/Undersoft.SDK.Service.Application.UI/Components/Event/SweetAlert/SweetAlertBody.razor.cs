using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class SweetAlertBody
{
    private string InternalCloseButtonText => IsConfirm ? CancelButtonText : CloseButtonText;

    [Inject]
    [NotNull]
    private IStringLocalizer<SweetAlert>? Localizer { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? CancelButtonText { get; set; }

    [Parameter]
    public SwalCategory Category { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public bool ShowClose { get; set; } = true;

    [Parameter]
    public bool ShowFooter { get; set; }

    [Parameter]
    public bool IsConfirm { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonIcon { get; set; }

    [Parameter]
    public Func<Task>? OnCloseAsync { get; set; }

    [Parameter]
    public Func<Task>? OnConfirmAsync { get; set; }

    [Parameter]
    public RenderFragment? BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public RenderFragment? ButtonTemplate { get; set; }

    [CascadingParameter]
    private Func<Task>? CloseModal { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? IconClassString => CssBuilder.Default("swal2-icon")
        .AddClass("swal2-success swal2-animate-success-icon", Category == SwalCategory.Success)
        .AddClass("swal2-error swal2-animate-error-icon", Category == SwalCategory.Error)
        .AddClass("swal2-info", Category == SwalCategory.Information)
        .AddClass("swal2-question", Category == SwalCategory.Question)
        .AddClass("swal2-warning", Category == SwalCategory.Warning)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        CancelButtonText ??= Localizer[nameof(CancelButtonText)];
        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];

        CloseButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.SweetAlertCloseIcon);
        ConfirmButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.SweetAlertConfirmIcon);
    }

    private async Task OnClickClose()
    {
        if (OnCloseAsync != null)
        {
            await OnCloseAsync();
        }

        if (CloseModal != null)
        {
            await CloseModal();
        }
    }

    private async Task OnClickConfirm()
    {
        if (OnConfirmAsync != null)
        {
            await OnConfirmAsync();
        }

        if (CloseModal != null)
        {
            await CloseModal();
        }
    }
}
