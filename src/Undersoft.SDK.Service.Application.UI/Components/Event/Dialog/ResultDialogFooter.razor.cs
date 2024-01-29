using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class ResultDialogFooter
{
    [Parameter]
    [NotNull]
    public bool ShowYesButton { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? ButtonYesText { get; set; }

    [Parameter]
    public string? ButtonYesIcon { get; set; }

    [Parameter] public Color ButtonYesColor { get; set; } = Color.Primary;

    [Parameter]
    [NotNull]
    public bool ShowNoButton { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? ButtonNoText { get; set; }

    [Parameter]
    [NotNull]
    public string? ButtonNoIcon { get; set; }

    [Parameter]
    public Color ButtonNoColor { get; set; } = Color.Danger;

    [Parameter]
    [NotNull]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? ButtonCloseText { get; set; }

    [Parameter]
    [NotNull]
    public string? ButtonCloseIcon { get; set; }

    [Parameter]
    public Color ButtonCloseColor { get; set; } = Color.Secondary;

    [Parameter]
    public Func<Task>? OnClickClose { get; set; }

    [Parameter]
    public Func<Task>? OnClickYes { get; set; }

    [Parameter]
    public Func<Task>? OnClickNo { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<ResultDialogOption>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ButtonCloseText ??= Localizer[nameof(ButtonCloseText)];
        ButtonNoText ??= Localizer[nameof(ButtonNoText)];
        ButtonYesText ??= Localizer[nameof(ButtonYesText)];

        ButtonYesIcon ??= IconTheme.GetIconByKey(ComponentIcons.ResultDialogYesIcon);
        ButtonNoIcon ??= IconTheme.GetIconByKey(ComponentIcons.ResultDialogNoIcon);
        ButtonCloseIcon ??= IconTheme.GetIconByKey(ComponentIcons.ResultDialogCloseIcon);
    }

    private async Task ButtonClick(DialogResult dialogResult)
    {
        if (dialogResult == DialogResult.Yes && OnClickYes != null)
        {
            await OnClickYes();
        }
        if (dialogResult == DialogResult.No && OnClickNo != null)
        {
            await OnClickNo();
        }
        if (dialogResult == DialogResult.Close && OnClickClose != null)
        {
            await OnClickClose();
        }
    }
}
