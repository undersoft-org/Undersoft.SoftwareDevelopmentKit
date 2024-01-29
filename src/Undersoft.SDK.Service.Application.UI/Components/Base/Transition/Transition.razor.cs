namespace Undersoft.SDK.Service.Application.Components;

public partial class Transition : IDisposable
{
    private ElementReference TransitionElement { get; set; }

    private JSInterop<Transition>? Interop { get; set; }

    private string? ClassString => CssBuilder
        .Default("animate__animated")
        .AddClass(TransitionType.ToDescriptionString(), Show)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"--animate-duration: {Duration / 1000}s", Duration > 100)
        .AddStyleFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public bool Show { get; set; } = true;

    [Parameter]
    public TransitionType TransitionType { get; set; } = TransitionType.FadeIn;

    [Parameter]
    public int Duration { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task>? OnTransitionEnd { get; set; }

    [JSInvokable]
    public async Task TransitionEndAsync()
    {
        if (OnTransitionEnd != null)
        {
            await OnTransitionEnd();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            Interop = new JSInterop<Transition>(JSRuntime);
            await Interop.InvokeVoidAsync(this, TransitionElement, "bb_transition", nameof(TransitionEndAsync));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Interop != null)
            {
                Interop.Dispose();
                Interop = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
