namespace Undersoft.SDK.Service.Application.Components;

public partial class Slider : IDisposable
{
    [Parameter]
    public double Value { get; set; }

    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

    [Parameter]
    public Func<double, Task>? OnValueChanged { get; set; }

    protected string? Disabled => IsDisabled ? "disabled" : null;

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public double Max { get; set; } = 100;

    [Parameter]
    public double Min { get; set; } = 0;

    private JSInterop<Slider>? Interop { get; set; }

    private string? ClassName => CssBuilder.Default("slider")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? SliderClassName => CssBuilder.Default("slider-runway")
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? BarStyle => CssBuilder.Default("left: 0%;")
        .AddClass($"width: {Value / Max * 100}%;")
        .Build();

    private string? ButtonStyle => CssBuilder.Default()
        .AddClass($"left: {Value / Max * 100}%;")
        .Build();

    private ElementReference SliderElement { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Interop = new JSInterop<Slider>(JSRuntime);
            await Interop.InvokeVoidAsync(this, SliderElement, "bb_slider", nameof(SetValue));
        }
    }

    [JSInvokable]
    public async Task SetValue(double val)
    {
        Value = Max * val / 100;
        if (OnValueChanged != null)
        {
            await OnValueChanged(Value);
        }

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
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
