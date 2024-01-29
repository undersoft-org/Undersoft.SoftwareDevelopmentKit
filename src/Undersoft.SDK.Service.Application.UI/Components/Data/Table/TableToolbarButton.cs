namespace Undersoft.SDK.Service.Application.Components;

[JSModuleNotInherited]
public class TableToolbarButton<TItem> : ButtonBase, ITableToolbarButton<TItem>
{
    [Parameter]
    public Func<IEnumerable<TItem>, Task>? OnClickCallback { get; set; }

    [Parameter]
    public bool IsEnableWhenSelectedOneRow { get; set; }

    [Parameter]
    public Func<IEnumerable<TItem>, bool>? IsDisabledCallback { get; set; }

    [Parameter]
    public bool IsShow { get; set; } = true;

    [CascadingParameter]
    protected TableToolbar<TItem>? Toolbar { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Toolbar?.AddButton(this);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        Toolbar?.RemoveButton(this);
        await base.DisposeAsync(disposing);
    }
}
