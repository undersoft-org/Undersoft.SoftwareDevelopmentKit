using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Chart : Component, IAsyncDisposable
{
    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private DotNetObjectReference<Chart>? Interop { get; set; }

    private ElementReference Element { get; set; }

    private string? ClassName => CssBuilder.Default("chart d-flex justify-content-center align-items-center position-relative is-loading")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"height: {Height};", !string.IsNullOrEmpty(Height))
        .AddClass($"width: {Width};", !string.IsNullOrEmpty(Width))
        .Build();

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Height { get; set; }

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public bool Responsive { get; set; } = true;

    [Parameter]
    public bool MaintainAspectRatio { get; set; } = true;

    [Parameter]
    public int AspectRatio { get; set; } = 2;

    [Parameter]
    public int ResizeDelay { get; set; } = 0;

    [Parameter]
    public int Angle { get; set; }

    [Parameter]
    public double? BorderWidth { get; set; }

    [Parameter]
    [NotNull]
    public string? LoadingText { get; set; }

    [Parameter]
    public ChartType ChartType { get; set; }

    [Parameter]
    public ChartAction ChartAction { get; set; }

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public Func<Task<ChartDataSource>>? OnInitAsync { get; set; }

    [Parameter]
    public Func<Task>? OnAfterInitAsync { get; set; }

    [Parameter]
    public Func<ChartAction, Task>? OnAfterUpdateAsync { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Chart>? Localizer { get; set; }

    private bool UpdateDataSource { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        LoadingText ??= Localizer[nameof(LoadingText)];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (OnInitAsync == null)
            {
                throw new InvalidOperationException("OnInit parameter must be set");
            }

            var ds = await OnInitAsync.Invoke();
            ds.Type ??= ChartType.ToDescriptionString();
            ds.Options.Title = ds.Options.Title ?? Title;
            ds.Options.Responsive = ds.Options.Responsive ?? Responsive;
            ds.Options.MaintainAspectRatio = ds.Options.MaintainAspectRatio ?? MaintainAspectRatio;
            ds.Options.AspectRatio = ds.Options.AspectRatio ?? AspectRatio;
            ds.Options.ResizeDelay = ds.Options.ResizeDelay ?? ResizeDelay;

            if (BorderWidth.HasValue)
            {
                ds.Options.BorderWidth = BorderWidth.Value;
            }
            if (Height != null && Width != null)
            {
                ds.Options.MaintainAspectRatio = false;
            }

            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.Chart/Components/Chart/Chart.razor.js");
            Interop = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("init", Element, Interop, nameof(Completed), ds);
        }
    }

    [JSInvokable]
    public void Completed()
    {
        OnAfterInitAsync?.Invoke();
    }

    public async Task Update(ChartAction action)
    {
        if (OnInitAsync != null)
        {
            var ds = await OnInitAsync();
            ds.Type ??= ChartType.ToDescriptionString();
            await Module.InvokeVoidAsync("update", Element, ds, action.ToDescriptionString(), Angle);

            if (OnAfterUpdateAsync != null)
            {
                await OnAfterUpdateAsync(action);
            }
        }
    }

    public Task Reload() => Update(ChartAction.Reload);

    #region Dispose
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
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
