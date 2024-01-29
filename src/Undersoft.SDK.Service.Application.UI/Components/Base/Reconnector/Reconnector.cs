namespace Undersoft.SDK.Service.Application.Components;

public class Reconnector : ComponentBase, IReconnector
{
    [Parameter]
    public RenderFragment? ReconnectingTemplate { get; set; }

    [Parameter]
    public RenderFragment? ReconnectFailedTemplate { get; set; }

    [Parameter]
    public RenderFragment? ReconnectRejectedTemplate { get; set; }

    [Inject]
    [NotNull]
    private IReconnectorProvider? Provider { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        Provider.NotifyContentChanged(this);
    }
}
