using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class BarcodeReader : IAsyncDisposable
{
    private string? AutoStopString => AutoStop ? "true" : null;

    private string? AutoStartString => AutoStart ? "true" : null;

    private bool IsDisabled { get; set; } = true;

    private string? SelectedDeviceId { get; set; }

    private string VideoId => $"video_{Id}";

    private string VideoStyleString => $"width: {VideoWidth}px; height: {VideoHeight}px;";

    [Parameter]
    [NotNull]
    public string? ButtonScanText { get; set; }

    [Parameter]
    [NotNull]
    public string? ButtonStopText { get; set; }

    [Parameter]
    [NotNull]
    public string? AutoStopText { get; set; }

    [Parameter]
    [NotNull]
    public string? DeviceLabel { get; set; }

    [Parameter]
    [NotNull]
    public string? InitDevicesString { get; set; }

    [Parameter]
    [NotNull]
    public string? NotFoundDevicesString { get; set; }

    [Parameter]
    public ScanType ScanType { get; set; }

    [Parameter]
    public int VideoWidth { get; set; } = 300;

    [Parameter]
    public int VideoHeight { get; set; } = 170;

    [Parameter]
    public Func<DeviceItem, Task>? OnDeviceChanged { get; set; }

    [Parameter]
    public Func<IEnumerable<DeviceItem>, Task>? OnInit { get; set; }

    [Parameter]
    public Func<string, Task>? OnResult { get; set; }

    [Parameter]
    public bool AutoStart { get; set; }

    [Parameter]
    public bool AutoStop { get; set; }

    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    [Parameter]
    public Func<Task>? OnStart { get; set; }

    [Parameter]
    public Func<Task>? OnClose { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<BarcodeReader>? Localizer { get; set; }

    private IEnumerable<SelectedItem>? Devices { get; set; }

    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private DotNetObjectReference<BarcodeReader>? Interop { get; set; }

    private ElementReference Element { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ButtonScanText ??= Localizer[nameof(ButtonScanText)];
        ButtonStopText ??= Localizer[nameof(ButtonStopText)];
        AutoStopText ??= Localizer[nameof(AutoStopText)];
        DeviceLabel ??= Localizer[nameof(DeviceLabel)];
        InitDevicesString ??= Localizer[nameof(InitDevicesString)];
        NotFoundDevicesString ??= Localizer[nameof(NotFoundDevicesString)];

        Devices ??= Enumerable.Empty<SelectedItem>();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.BarCode/Components/BarcodeReader/BarcodeReader.razor.js");
            Interop = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("init", Element, Interop);
        }
    }

    [JSInvokable]
    public async Task InitDevices(IEnumerable<DeviceItem> devices)
    {
        Devices = devices.Select(i => new SelectedItem { Value = i.DeviceId, Text = i.Label });
        IsDisabled = !Devices.Any();

        if (OnInit != null) await OnInit(devices);
        if (IsDisabled) InitDevicesString = NotFoundDevicesString;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task GetResult(string val)
    {
        if (OnResult != null) await OnResult(val);
    }

    [JSInvokable]
    public async Task GetError(string err)
    {
        if (OnError != null) await OnError(err);
    }

    [JSInvokable]
    public async Task Start()
    {
        if (OnStart != null) await OnStart();
    }

    [JSInvokable]
    public async Task Stop()
    {
        if (OnClose != null) await OnClose();
    }

    private async Task OnSelectedItemChanged(SelectedItem item)
    {
        SelectedDeviceId = item.Value;

        if (OnDeviceChanged != null)
        {
            await OnDeviceChanged(new DeviceItem() { DeviceId = item.Value, Label = item.Text });
        }
        StateHasChanged();
    }

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
