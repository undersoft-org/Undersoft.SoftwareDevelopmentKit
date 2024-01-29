using System.Collections.Concurrent;

namespace Undersoft.SDK.Service.Application.Components;

#if NET6_0_OR_GREATER
[CascadingTypeParameter(nameof(TItem))]
#endif
public partial class TableToolbar<TItem> : ComponentBase
{
    private List<ButtonBase> Buttons { get; } = new();

    private readonly ConcurrentDictionary<ButtonBase, bool> _asyncButtonStateCache = new();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [NotNull]
    public Func<IEnumerable<TItem>>? OnGetSelectedRows { get; set; }

    [Parameter]
    public bool IsAutoCollapsedToolbarButton { get; set; } = true;

    [Parameter]
    public string? GearIcon { get; set; }

    private string? ToolbarClassString => CssBuilder.Default("btn-toolbar btn-group")
        .AddClass("d-none d-sm-inline-flex", IsAutoCollapsedToolbarButton)
        .Build();

    private async Task OnToolbarButtonClick(TableToolbarButton<TItem> button)
    {
        _asyncButtonStateCache.TryGetValue(button, out var disabled);
        if (!disabled)
        {
            _asyncButtonStateCache.TryAdd(button, true);
            if (button.OnClick.HasDelegate)
            {
                await button.OnClick.InvokeAsync();
            }

            if (button.OnClickCallback != null)
            {
                await button.OnClickCallback(OnGetSelectedRows());
            }
            _asyncButtonStateCache.TryRemove(button, out _);
        }
    }

    private async Task OnConfirm(TableToolbarPopconfirmButton<TItem> button)
    {
        _asyncButtonStateCache.TryGetValue(button, out var disabled);
        if (!disabled)
        {
            _asyncButtonStateCache.TryAdd(button, true);
            if (button.OnClick.HasDelegate)
            {
                await button.OnClick.InvokeAsync();
            }

            await button.OnConfirm();

            if (button.OnConfirmCallback != null)
            {
                await button.OnConfirmCallback(OnGetSelectedRows());
            }
            _asyncButtonStateCache.TryRemove(button, out _);
        }
    }

    private bool GetDisabled(ButtonBase button)
    {
        var ret = button.IsDisabled;
        if (button.IsAsync && _asyncButtonStateCache.TryGetValue(button, out var b))
        {
            ret = b;
        }
        else if (button is ITableToolbarButton<TItem> tb)
        {
            ret = tb.IsDisabledCallback == null ? (tb.IsEnableWhenSelectedOneRow && OnGetSelectedRows().Count() != 1) : tb.IsDisabledCallback(OnGetSelectedRows());
        }
        return ret;
    }

    public void AddButton(ButtonBase button) => Buttons.Add(button);

    public void RemoveButton(ButtonBase button) => Buttons.Remove(button);
}
