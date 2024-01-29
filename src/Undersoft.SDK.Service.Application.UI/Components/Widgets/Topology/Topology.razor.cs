using Microsoft.AspNetCore.Components;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Topology : IAsyncDisposable
{
    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? Content { get; set; }

    [Parameter]
    public int Interval { get; set; } = 2000;

    [Parameter]
    public Func<CancellationToken, Task<IEnumerable<TopologyItem>>>? OnQueryAsync { get; set; }

    [Parameter]
    public Func<Task>? OnBeforePushData { get; set; }

    [Parameter]
    public bool IsSupportTouch { get; set; }

    [Parameter]
    public bool IsFitView { get; set; }

    [Parameter]
    public bool IsCenterView { get; set; }

    private string? StyleString => CssBuilder.Default("width: 100%; height: 100%;")
        .AddStyleFromAttributes(AdditionalAttributes)
        .Build();

    private CancellationTokenSource? CancelToken { get; set; }

    private string? IsSupportTouchString => IsSupportTouch ? "true" : null;

    private string? IsFitViewString => IsFitView ? "true" : null;

    private string? IsCenterViewString => IsCenterView ? "true" : null;

    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private DotNetObjectReference<Topology>? Interop { get; set; }

    private ElementReference Element { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.Topology/Components/Topology/Topology.razor.js");
            Interop = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("init", Element, Interop, Content, nameof(PushData));
        }
    }

    [JSInvokable]
    public async Task PushData()
    {
        if (!_disposing)
        {
            if (OnBeforePushData != null)
            {
                await OnBeforePushData();
            }

            if (OnQueryAsync != null)
            {
                Interval = Math.Max(100, Interval);
                CancelToken = new CancellationTokenSource();
                while (CancelToken != null && !CancelToken.IsCancellationRequested)
                {
                    try
                    {
                        var data = await OnQueryAsync(CancelToken.Token);
                        await PushData(data);
                        await Task.Delay(Interval, CancelToken.Token);
                    }
                    catch (TaskCanceledException)
                    {

                    }
                }
            }
        }
    }

    public async ValueTask PushData(IEnumerable<TopologyItem> items)
    {
        if (!_disposing)
        {
            await Module.InvokeVoidAsync("update", Element, items);
        }
    }

    public ValueTask Scale(int rate = 1) => Module.InvokeVoidAsync("scale", Element, rate);

    public ValueTask Reset() => Module.InvokeVoidAsync("reset", Element);

    public ValueTask Resize(int? width = null, int? height = null) => Module.InvokeVoidAsync("resize", Element, width, height);

    #region Dispose
    private bool _disposing;

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        _disposing = true;
        if (disposing)
        {
            if (CancelToken != null)
            {
                CancelToken.Cancel();
                CancelToken.Dispose();
                CancelToken = null;
            }

            Interop?.Dispose();

            if (Module != null)
            {
                await Module.InvokeVoidAsync("dispose", Element);
                await Module.DisposeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
