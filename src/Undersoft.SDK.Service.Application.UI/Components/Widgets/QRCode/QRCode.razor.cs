using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class QRCode : IAsyncDisposable
{
    private string? ClassString => CssBuilder.Default("qrcode")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ImageStyleString => $"width: {Width}px; height: {Width}px;";

    [Parameter]
    public Func<Task>? OnGenerated { get; set; }

    [Parameter]
    [NotNull]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public bool ShowButtons { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    [Parameter]
    public string? ClearButtonIcon { get; set; } = "fa-solid fa-xmark";

    [Parameter]
    [NotNull]
    public string? GenerateButtonText { get; set; }

    [Parameter]
    public string? LightColor { get; set; } = "#ffffff";

    [Parameter]
    [NotNull]
    public string? DarkColor { get; set; } = "#000000";

    [Parameter]
    public string? GenerateButtonIcon { get; set; } = "fa-solid fa-qrcode";

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public int Width { get; set; } = 128;

    [Inject]
    [NotNull]
    private IStringLocalizer<QRCode>? Localizer { get; set; }

    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private DotNetObjectReference<QRCode>? Interop { get; set; }

    private ElementReference Element { get; set; }

    private string? _content;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Width = Math.Max(40, Width);
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        GenerateButtonText ??= Localizer[nameof(GenerateButtonText)];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.BarCode/Components/QRCode/QRCode.razor.js");
            Interop = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("init", Element, Interop, Content, nameof(Generated));
        }
        else
        {
            await Generate();
        }
    }


    private async Task Clear()
    {
        Content = "";
        await Generate();
    }

    private async Task Generate()
    {
        if (_content != Content)
        {
            _content = Content;
            await Module.InvokeVoidAsync("update", Element, Content);
        }
    }

    [JSInvokable]
    public async Task Generated()
    {
        if (OnGenerated != null)
        {
            await OnGenerated();
        }
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
