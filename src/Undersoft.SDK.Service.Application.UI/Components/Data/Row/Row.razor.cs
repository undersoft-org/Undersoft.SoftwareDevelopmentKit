using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Row
{
    [Parameter]
    public ItemsPerRow ItemsPerRow { get; set; }

    [Parameter]
    public RowType RowType { get; set; }

    [Parameter]
    public int? ColSpan { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private ElementReference RowElement { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync(RowElement, "bb_row");
        }
    }
}
