using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class DialogSaveButton : Button
{
    [Inject]
    [NotNull]
    private IStringLocalizer<ModalDialog>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ButtonType = ButtonType.Submit;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogSaveButtonIcon);
        Text ??= Localizer[nameof(ModalDialog.SaveButtonText)];
    }
}
