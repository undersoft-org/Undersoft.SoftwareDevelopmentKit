namespace Undersoft.SDK.Service.Application.Components;

public partial class Carousel
{
    private string? ClassName => CssBuilder.Default("carousel slide")
        .AddClass("carousel-fade", IsFade)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? TargetId => $"#{Id}";

    private string? StyleName => CssBuilder.Default()
        .AddClass($"width: {Width.ConvertToPercentString()};", !string.IsNullOrEmpty(Width))
        .Build();

    private static string? CheckActive(int index, string? css = null) => CssBuilder.Default(css)
        .AddClass("active", index == 0)
        .Build();

    [Parameter]
    public IEnumerable<string> Images { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public bool IsFade { get; set; }

    [Parameter]
    public Func<string, Task>? OnClick { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool ShowControls { get; set; } = true;

    [Parameter]
    public bool ShowIndicators { get; set; } = true;

    [Parameter]
    public bool DisableTouchSwiping { get; set; }

    [Parameter]
    public string? PreviousIcon { get; set; }

    [Parameter]
    public string? NextIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? DisableTouchSwipingString => DisableTouchSwiping ? "false" : null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PreviousIcon ??= IconTheme.GetIconByKey(ComponentIcons.CarouselPreviousIcon);
        NextIcon ??= IconTheme.GetIconByKey(ComponentIcons.CarouselNextIcon);

        if (Items.Count == 0)
        {
            foreach (var image in Images)
            {
                var item = new CarouselItem();
#if NET5_0
                item.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object>()
#else
                item.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>()
#endif
                {
                    [nameof(CarouselItem.ChildContent)] = new RenderFragment(builder =>
                    {
                        builder.OpenComponent<CarouselImage>(0);
                        builder.AddAttribute(1, nameof(CarouselImage.ImageUrl), image);
                        if (OnClick != null)
                        {
                            builder.AddAttribute(2, nameof(CarouselImage.OnClick), OnClickImage);
                        }
                        builder.CloseComponent();
                    })
                }));
                Items.Add(item);
            }
        }
    }

    protected async Task OnClickImage(string imageUrl)
    {
        if (OnClick != null) await OnClick(imageUrl);
    }

    private List<CarouselItem> Items { get; } = new(10);

    internal void AddItem(CarouselItem item) => Items.Add(item);

    internal void RemoveItem(CarouselItem item) => Items.Remove(item);
}
