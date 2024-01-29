using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Text;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Camera : IAsyncDisposable
{
    private ElementReference CameraElement { get; set; }

    private JSInterop<Camera>? Interop { get; set; }

    private string DeviceId { get; set; } = "";

    private bool IsDisabled { get; set; } = true;

    private bool CaptureDisabled { get; set; } = true;

    private IEnumerable<SelectedItem> Devices { get; set; } = Enumerable.Empty<SelectedItem>();

    [NotNull]
    private IEnumerable<SelectedItem>? Cameras { get; set; }

    private SelectedItem? ActiveCamera { get; set; }

    [Parameter]
    public bool AutoStart { get; set; }

    [Parameter]
    [NotNull]
    public string? FrontText { get; set; }

    [Parameter]
    [NotNull]
    public string? BackText { get; set; }

    [Parameter]
    public bool ShowPreview { get; set; }

    [Parameter]
    [NotNull]
    public string? DeviceLabel { get; set; }

    [Parameter]
    [NotNull]
    public string? InitDevicesString { get; set; }

    [Parameter]
    public int VideoWidth { get; set; } = 320;

    [Parameter]
    public int VideoHeight { get; set; } = 240;

    [Parameter]
    public Func<IEnumerable<DeviceItem>, Task>? OnInit { get; set; }

    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    [Parameter]
    public Func<Task>? OnStart { get; set; }

    [Parameter]
    public Func<Task>? OnClose { get; set; }

    [Parameter]
    public Func<string, Task>? OnCapture { get; set; }

    [Parameter]
    [NotNull]
    public string? PlayIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? StopIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? PhotoIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? PlayText { get; set; }

    [Parameter]
    [NotNull]
    public string? StopText { get; set; }

    [Parameter]
    [NotNull]
    public string? PhotoText { get; set; }

    [Parameter]
    [NotNull]
    public string? NotFoundDevicesString { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Camera>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string VideoWidthString => $"{VideoWidth}px;";

    private string VideoHeightString => $"{VideoHeight}px;";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        PlayText ??= Localizer[nameof(PlayText)];
        StopText ??= Localizer[nameof(StopText)];
        PhotoText ??= Localizer[nameof(PhotoText)];
        DeviceLabel ??= Localizer[nameof(DeviceLabel)];
        InitDevicesString ??= Localizer[nameof(InitDevicesString)];
        NotFoundDevicesString ??= Localizer[nameof(NotFoundDevicesString)];
        FrontText ??= Localizer[nameof(FrontText)];
        BackText ??= Localizer[nameof(BackText)];

        Cameras = new SelectedItem[]
        {
            new SelectedItem { Text = FrontText!, Value = "user", Active = true },
            new SelectedItem { Text = BackText!, Value = "environment" }
        };
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PlayIcon ??= IconTheme.GetIconByKey(ComponentIcons.CameraPlayIcon);
        StopIcon ??= IconTheme.GetIconByKey(ComponentIcons.CameraStopIcon);
        PhotoIcon ??= IconTheme.GetIconByKey(ComponentIcons.CameraPhotoIcon);

        if (VideoWidth < 40)
        {
            VideoWidth = 40;
        }

        if (VideoHeight < 30)
        {
            VideoHeight = 30;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Interop = new JSInterop<Camera>(JSRuntime);
            await Interop.InvokeVoidAsync(this, CameraElement, "bb_camera", "init", AutoStart, VideoWidth, VideoHeight);
        }
    }

    [JSInvokable]
    public async Task InitDevices(IEnumerable<DeviceItem> devices)
    {
        Devices = devices.Select(i => new SelectedItem { Value = i.DeviceId, Text = i.Label });
        IsDisabled = !Devices.Any();

        if (OnInit != null)
        {
            await OnInit(devices);
        }

        if (devices.Any())
        {
            for (var index = 0; index < devices.Count(); index++)
            {
                var d = devices.ElementAt(index);
                if (string.IsNullOrEmpty(d.Label))
                {
                    d.Label = $"Video device {index + 1}";
                }
            }
            ActiveCamera = Cameras.First();
        }

        if (IsDisabled)
        {
            InitDevicesString = NotFoundDevicesString;
        }

        StateHasChanged();
    }

    [JSInvokable]
    public async Task GetError(string err)
    {
        if (OnError != null)
        {
            await OnError.Invoke(err);
        }
    }

    [JSInvokable]
    public async Task Start()
    {
        CaptureDisabled = false;
        if (OnStart != null)
        {
            await OnStart.Invoke();
        }

        StateHasChanged();
    }

    [JSInvokable]
    public async Task Stop()
    {
        CaptureDisabled = true;
        if (OnClose != null)
        {
            await OnClose.Invoke();
        }

        StateHasChanged();
    }

    private readonly StringBuilder _sb = new();
    [JSInvokable]
    public async Task Capture(string payload)
    {
        if (payload == "__BB__%END%__BB__")
        {
            var data = _sb.ToString();
            _sb.Clear();
            if (OnCapture != null)
            {
                await OnCapture(data);
            }
        }
        else
        {
            _sb.Append(payload);
        }
    }

    protected virtual async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            if (Interop != null)
            {
                await JSRuntime.InvokeVoidAsync(CameraElement, "bb_camera", "", "stop").ConfigureAwait(false);
                Interop.Dispose();
                Interop = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
