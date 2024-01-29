using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class DialogCloseButton : Button
{
    [Parameter]
    public override Color Color { get; set; } = Color.Secondary;

    [CascadingParameter]
    private Func<Task>? OnCloseAsync { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<ModalDialog>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogCloseButtonIcon);
        Text ??= Localizer[nameof(ModalDialog.CloseButtonText)];
    }

    protected override async Task HandlerClick()
    {
        await base.HandlerClick();

        if (OnCloseAsync != null)
        {
            await OnCloseAsync();
        }
    }
}
