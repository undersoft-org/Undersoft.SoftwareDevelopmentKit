using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Handwritten : IDisposable
{
    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Handwritten>? Localizer { get; set; }

    [Parameter]
    public EventCallback<string> HandwrittenBase64 { get; set; }

    [Parameter]
    public string? Result { get; set; }

    [NotNull]
    private JSInterop<Handwritten>? Interop { get; set; }

    private ElementReference HandwrittenElement { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        SaveButtonText ??= Localizer[nameof(SaveButtonText)];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Interop ??= new JSInterop<Handwritten>(JSRuntime);
            await Interop.InvokeVoidAsync(this, HandwrittenElement, "bb_handwritten", true, nameof(OnValueChanged));
        }
    }

    [JSInvokable]
    public async Task OnValueChanged(string val)
    {
        Result = val;
        StateHasChanged();
        await HandwrittenBase64.InvokeAsync(val);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Interop?.Dispose();
            Interop = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
