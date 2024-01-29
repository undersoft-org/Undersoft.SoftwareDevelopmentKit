namespace Undersoft.SDK.Service.Application.Components;

public partial class Progress
{
    [Parameter]
    public int? Height { get; set; }

    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public bool IsShowValue { get; set; }

    [Parameter]
    public bool IsStriped { get; set; }

    [Parameter]
    public bool IsAnimated { get; set; }

    [Parameter]
    public double Value { get; set; }

    [Parameter]
    public int Round { get; set; }

    [Parameter]
    public MidpointRounding MidpointRounding { get; set; } = MidpointRounding.AwayFromZero;

    [Parameter]
    public string? Text { get; set; }

    private string? ClassString => CssBuilder.Default("progress")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ClassName => CssBuilder.Default("progress-bar")
        .AddClass($"bg-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("progress-bar-striped", IsStriped)
        .AddClass("progress-bar-animated", IsAnimated)
        .Build();

    private string? StyleName => CssBuilder.Default()
        .AddClass($"width: {InternalValue}%;")
        .Build();

    private string? ProgressStyle => CssBuilder.Default()
        .AddClass($"height: {Height}px;", Height.HasValue)
        .Build();

    private double InternalValue => Round == 0 ? Value : Math.Round(Value, Round, MidpointRounding);

    private string? ValueLabelString => IsShowValue ? string.IsNullOrEmpty(Text) ? $"{InternalValue}%" : Text : null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Value = Math.Min(100, Math.Max(0, Value));
    }
}
