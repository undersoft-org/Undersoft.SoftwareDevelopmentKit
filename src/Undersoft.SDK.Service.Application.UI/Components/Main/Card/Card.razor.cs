namespace Undersoft.SDK.Service.Application.Components;

public partial class Card
{
    protected string? ClassString => CssBuilder.Default("card")
        .AddClass("text-center", IsCenter)
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("card-shadow", IsShadow)
        .AddClass("is-collapsable", IsCollapsible)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? ArrowClassString => CssBuilder.Default("card-collapse-arrow")
        .AddClass(CollapseIcon)
        .Build();

    protected string? BodyClassName => CssBuilder.Default("card-body")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("collapse", IsCollapsible && Collapsed)
        .AddClass("collapse show", IsCollapsible && !Collapsed)
        .Build();

    protected string? ExpandedString => Collapsed ? "false" : "true";

    protected string? FooterClassName => CssBuilder.Default("card-footer")
        .AddClass("text-muted", IsCenter)
        .Build();

    [Parameter]
    public string? CollapseIcon { get; set; }

    [Parameter]
    public string? HeaderText { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public bool IsCenter { get; set; }

    [Parameter]
    public bool IsCollapsible { get; set; }

    [Parameter]
    public bool Collapsed { get; set; }

    [Parameter]
    public bool IsShadow { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        CollapseIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardCollapseIcon);
    }
}
