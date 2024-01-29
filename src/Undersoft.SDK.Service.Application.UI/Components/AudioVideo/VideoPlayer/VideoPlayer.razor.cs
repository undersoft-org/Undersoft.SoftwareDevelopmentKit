using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

                                                                                                                                                                 public partial class VideoPlayer : IAsyncDisposable
{
    [Inject]
    [NotNull]
    private IJSRuntime? JSRuntime { get; set; }

    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private IJSObjectReference? ModuleLang { get; set; }

    private DotNetObjectReference<VideoPlayer>? Instance { get; set; }

    private ElementReference Element { get; set; }

    private bool IsInitialized { get; set; }

    private string? DebugInfo { get; set; }

    [NotNull]
    private string? Id { get; set; }

     [Parameter]
    [NotNull]
    [EditorRequired]
    public string? Url { get; set; }

     [Parameter]
    [NotNull]
    public string? MineType { get; set; } = "application/x-mpegURL";

     [Parameter]
    public int Width { get; set; } = 300;

     [Parameter]
    public int Height { get; set; } = 200;

     [Parameter]
    public bool Controls { get; set; } = true;

     [Parameter]
    public bool Autoplay { get; set; } = true;

     [Parameter]
    public string Preload { get; set; } = "auto";

     [Parameter]
    public string? Poster { get; set; }

     [Parameter]
    public string? Language { get; set; }

     [Parameter]
    public bool Debug { get; set; }

     [Parameter]
    public Func<string, Task>? OnError { get; set; }

     protected override void OnInitialized()
    {
        base.OnInitialized();

        Id = $"vp_{GetHashCode()}";
    }

    string? Ver { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    string? CssPath { get => "./_content/BootstrapBlazor.VideoPlayer/video-js.min.css" + "?v=" + Ver; }

     protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.VideoPlayer/app.js" + "?v=" + Ver);

            Language = Language ?? CultureInfo.CurrentCulture.Name;
            try
            {
                await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/BootstrapBlazor.VideoPlayer/lang/{Language}.js" + "?v=" + Ver);
            }
            catch{
                try
                {
                    Language = Language.Contains("-") ? Language.Split("-")[0] : "zh-CN";
                    await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/BootstrapBlazor.VideoPlayer/lang/{Language}.js" + "?v=" + Ver);
                }
                catch
                {
                    Language = "zh-CN";
                    await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/BootstrapBlazor.VideoPlayer/lang/{Language}.js" + "?v=" + Ver);
                }
            }

            Instance = DotNetObjectReference.Create(this);
            await MakesurePlayerReady();
        }
    }

     private async Task MakesurePlayerReady()
    {
        if (!IsInitialized)
        {
            if (string.IsNullOrEmpty(Url))
            {
                await Logger($"Url is empty");
            }
            else
            {
                var option = new VideoPlayerOption()
                {
                    Width = Width,
                    Height = Height,
                    Controls = Controls,
                    Autoplay = Autoplay,
                    Preload = Preload,
                    Poster = Poster,
                    Language = Language,
                };
                option.Sources.Add(new VideoSources(MineType, Url));
                await Module.InvokeVoidAsync("loadPlayer", Instance, Id, option);
            }
        }
    }

     public virtual async Task Reload(string url, string mineType)
    {
        Url = url;
        MineType = mineType;
        await MakesurePlayerReady();
        await Module.InvokeVoidAsync("reloadPlayer", url, mineType);
    }

     public virtual async Task SetPoster(string poster)
    {
        Poster = poster;
        await Module.InvokeVoidAsync("setPoster", poster);
    }

     [JSInvokable]
    public void GetInit() => IsInitialized = true;

     [JSInvokable]
    public async Task Logger(string message)
    {
        DebugInfo = message;
        if (Debug)
        {
            StateHasChanged();
        }

        System.Console.WriteLine(DebugInfo);
        if (OnError != null)
        {
            await OnError.Invoke(DebugInfo);
        }
    }

     public async ValueTask DisposeAsync()
    {
        if (Module is not null)
        {
            await Module.InvokeVoidAsync("destroy", Id);
            await Module.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
