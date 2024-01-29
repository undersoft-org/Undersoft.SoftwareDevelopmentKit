namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/BootstrapBlazor/modules/responsive.js", JSObjectReference = true, Relative = false)]
public class ResizeNotification : ComponentModule
{
    [Inject]
    [NotNull]
    private ResizeNotificationService? ResizeService { get; set; }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, nameof(OnResize));

    [JSInvokable]
    public Task OnResize(BreakPoint point) => ResizeService.InvokeAsync(point);
}
