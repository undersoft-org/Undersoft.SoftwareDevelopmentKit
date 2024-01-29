namespace Undersoft.SDK.Service.Application.Components;

internal class ReconnectorProvider : IReconnectorProvider
{
    private Action<RenderFragment?, RenderFragment?, RenderFragment?>? _action;

    public void NotifyContentChanged(IReconnector reconnector)
    {
        _action?.Invoke(reconnector.ReconnectingTemplate, reconnector.ReconnectFailedTemplate, reconnector.ReconnectRejectedTemplate);
    }

    public void Register(Action<RenderFragment?, RenderFragment?, RenderFragment?> action) => _action = action;
}
