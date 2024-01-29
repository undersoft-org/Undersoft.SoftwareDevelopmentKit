namespace Undersoft.SDK.Service.Application.Components;

[JSModuleNotInherited]
public partial class TableExtensionButton
{
    private List<ButtonBase> Buttons { get; } = new();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<TableCellButtonArgs, Task>? OnClickButton { get; set; }

    public void AddButton(ButtonBase button) => Buttons.Add(button);

    public void RemoveButton(ButtonBase button) => Buttons.Remove(button);

    private async Task OnClick(TableCellButton b)
    {
        if (b.OnClick.HasDelegate)
        {
            await b.OnClick.InvokeAsync();
        }
        if (b.OnClickWithoutRender != null)
        {
            await b.OnClickWithoutRender();
        }

        if (OnClickButton != null)
        {
            await OnClickButton(new TableCellButtonArgs()
            {
                AutoRenderTableWhenClick = b.AutoRenderTableWhenClick,
                AutoSelectedRowWhenClick = b.AutoSelectedRowWhenClick
            });
        }
    }

    private async Task OnClickConfirm(TableCellPopconfirmButton b)
    {
        await b.OnConfirm();

        if (OnClickButton != null)
        {
            await OnClickButton(new TableCellButtonArgs()
            {
                AutoRenderTableWhenClick = b.AutoRenderTableWhenClick,
                AutoSelectedRowWhenClick = b.AutoSelectedRowWhenClick
            });
        }
    }
}
