using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleNotInherited]
public class TableCellPopconfirmButton : PopConfirmButtonBase
{
    [CascadingParameter]
    protected TableExtensionButton? Buttons { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<PopConfirmButton>? Localizer { get; set; }

    [Parameter]
    public bool AutoSelectedRowWhenClick { get; set; } = true;

    [Parameter]
    public bool AutoRenderTableWhenClick { get; set; }

    [Parameter]
    public bool IsShow { get; set; } = true;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Buttons?.AddButton(this);

        if (Size == Size.None)
        {
            Size = Size.ExtraSmall;
        }

        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];
        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        Content ??= Localizer[nameof(Content)];
    }

    protected override ValueTask DisposeAsync(bool disposing)
    {
        Buttons?.RemoveButton(this);
        return base.DisposeAsync(disposing);
    }
}
