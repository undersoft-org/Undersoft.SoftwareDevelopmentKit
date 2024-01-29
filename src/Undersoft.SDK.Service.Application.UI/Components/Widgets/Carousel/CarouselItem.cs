namespace Undersoft.SDK.Service.Application.Components;

public class CarouselItem : ComponentBase, IDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Caption { get; set; }

    [Parameter]
    public string? CaptionClass { get; set; }

    [Parameter]
    public RenderFragment? CaptionTemplate { get; set; }

    [Parameter]
    public int Interval { get; set; } = 5000;

    [CascadingParameter]
    private Carousel? Carousel { get; set; }

    internal string? IntervalString => Interval == 5000 ? null : Interval.ToString();

    protected override void OnInitialized()
    {
        Carousel?.AddItem(this);
    }

    public string? GetCaptionClassString => CssBuilder.Default("carousel-caption")
        .AddClass(CaptionClass)
        .Build();

    internal bool ShowCaption => CaptionTemplate != null || !string.IsNullOrEmpty(Caption);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Carousel?.RemoveItem(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
