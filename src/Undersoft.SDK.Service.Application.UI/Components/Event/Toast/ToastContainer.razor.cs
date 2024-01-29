namespace Undersoft.SDK.Service.Application.Components;

public partial class ToastContainer : IDisposable
{
    private string? ClassString => CssBuilder.Default("toast-container")
        .AddClass("top-0 start-0", Placement == Placement.TopStart)
        .AddClass("top-0 start-50 translate-middle-x", Placement == Placement.TopCenter)
        .AddClass("top-0 end-0", Placement == Placement.TopEnd)
        .AddClass("top-50 start-0 translate-middle-y", Placement == Placement.MiddleStart)
        .AddClass("top-50 start-50 translate-middle", Placement == Placement.MiddleCenter)
        .AddClass("top-50 end-0 translate-middle-y", Placement == Placement.MiddleEnd)
        .AddClass("bottom-0 start-0", Placement == Placement.BottomStart)
        .AddClass("bottom-0 start-50 translate-middle-x", Placement == Placement.BottomCenter)
        .AddClass("bottom-0 end-0", Placement == Placement.BottomEnd)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ToastBoxClassString => CssBuilder.Default()
        .AddClass("left", Placement == Placement.TopStart)
        .AddClass("left", Placement == Placement.MiddleStart)
        .AddClass("left", Placement == Placement.BottomStart)
        .AddClass("left", Placement == Placement.TopCenter)
        .AddClass("left", Placement == Placement.MiddleCenter)
        .AddClass("left", Placement == Placement.BottomCenter)
        .Build();

    private List<ToastOption> Toasts { get; } = new List<ToastOption>();

    [Parameter]
    [NotNull]
    public Placement Placement { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<ApplicationOptions>? Options { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Placement = Options.CurrentValue.ToastPlacement ?? Placement.BottomEnd;

        if (ToastService != null)
        {
            ToastService.Register(this, Show);
        }
    }

    private async Task Show(ToastOption option)
    {
        Toasts.Add(option);
        await InvokeAsync(StateHasChanged);
    }

    public void Close(ToastOption option)
    {
        Toasts.Remove(option);
        StateHasChanged();
    }

    public void SetPlacement(Placement placement)
    {
        Placement = placement;
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ToastService.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
