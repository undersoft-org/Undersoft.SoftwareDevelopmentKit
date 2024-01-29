namespace Undersoft.SDK.Service.Application.Components;

[JSModuleNotInherited]
public class TableCellButton : ButtonBase
{
    [CascadingParameter]
    protected TableExtensionButton? Buttons { get; set; }

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
    }

    protected override ValueTask DisposeAsync(bool disposing)
    {
        Buttons?.RemoveButton(this);
        return base.DisposeAsync(disposing);
    }
}
