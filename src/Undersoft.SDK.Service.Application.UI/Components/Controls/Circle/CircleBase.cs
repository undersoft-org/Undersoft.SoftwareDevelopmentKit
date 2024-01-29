namespace Undersoft.SDK.Service.Application.Components;

public abstract class CircleBase : ComponentModule2
{
    protected virtual string? ClassString => CssBuilder.Default("circle")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? PrevStyleString => CssBuilder.Default()
        .AddClass($"width: {Width}px;", Width > 0)
        .AddClass($"height: {Width}px;", Width > 0)
        .AddClass("transform: rotate(-90deg);")
        .Build();

    protected string? ProgressClassString => CssBuilder.Default("circle-progress")
        .AddClass($"circle-{Color.ToDescriptionString()}")
        .Build();

    protected string DashString => $"{CircleLength}, {CircleLength}";

    protected string CircleDiameter => $"{Width / 2}";

    protected string CircleR => $"{Width / 2 - StrokeWidth}";

    protected double CircleLength => Math.Round((Width / 2 - StrokeWidth) * 2 * Math.PI, 2);

    [Parameter]
    public virtual int Width { get; set; } = 120;

    [Parameter]
    public virtual int StrokeWidth { get; set; } = 2;

    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public bool ShowProgress { get; set; } = true;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Width / 2 < StrokeWidth) StrokeWidth = 2;
        Width = Math.Max(6, Width);
    }
}
