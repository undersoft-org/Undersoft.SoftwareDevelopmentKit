namespace Undersoft.SDK.Service.Application.Components;
using Undersoft.SDK.Service.Application.Extensions;

public partial class ReconnectorContent
{
    [Parameter]
    public RenderFragment? ReconnectingTemplate { get; set; }

    [Parameter]
    public RenderFragment? ReconnectFailedTemplate { get; set; }

    [Parameter]
    public RenderFragment? ReconnectRejectedTemplate { get; set; }

    [Parameter]
    public bool AutoReconnect { get; set; } = true;

    [Inject]
    [NotNull]
    private IReconnectorProvider? Provider { get; set; }

    [Inject]
    [NotNull]
    private IJSRuntime? JSRuntime { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Provider.Register(ContentChanged);
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && AutoReconnect)
        {
            await JSRuntime.InvokeVoidAsync(func: "bb_reconnect");
        }
    }

    private void ContentChanged(RenderFragment? reconnectingTemplate, RenderFragment? reconnectFailedTemplate, RenderFragment? reconnectRejectedTemplate)
    {
        ReconnectingTemplate = reconnectingTemplate;
        ReconnectFailedTemplate = reconnectFailedTemplate;
        ReconnectRejectedTemplate = reconnectRejectedTemplate;
        InvokeAsync(StateHasChanged);
    }
}
