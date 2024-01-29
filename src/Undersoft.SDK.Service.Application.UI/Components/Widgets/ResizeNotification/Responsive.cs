namespace Undersoft.SDK.Service.Application.Components;

public class Responsive : Component, IDisposable
{
    [Inject]
    [NotNull]
    private ResizeNotificationService? ResizeService { get; set; }

    [Parameter]
    public Func<BreakPoint, Task>? OnBreakPointChanged { get; set; }

    protected override void OnInitialized()
    {
        ResizeService.Subscribe(this, OnResize);
    }

    private async Task OnResize(BreakPoint point)
    {
        if (OnBreakPointChanged != null)
        {
            await OnBreakPointChanged(point);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ResizeService.Unsubscribe(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
