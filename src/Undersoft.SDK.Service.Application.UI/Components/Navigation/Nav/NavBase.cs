using Microsoft.AspNetCore.Components.Routing;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class NavBase : Component
{
    protected string? ClassString => CssBuilder.Default("nav")
        .AddClass("justify-content-center", Alignment == Alignment.Center && !IsVertical)
        .AddClass("justify-content-end", Alignment == Alignment.Right && !IsVertical)
        .AddClass("flex-column", IsVertical)
        .AddClass("nav-pills", IsPills)
        .AddClass("nav-fill", IsFill)
        .AddClass("nav-justified", IsJustified)
        .AddClass("text-end", Alignment == Alignment.Right && IsVertical)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    [NotNull]
    public IEnumerable<NavLink>? Items { get; set; }

    [Parameter]
    public Alignment Alignment { get; set; } = Alignment.Left;

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public bool IsPills { get; set; }

    [Parameter]
    public bool IsFill { get; set; }

    [Parameter]
    public bool IsJustified { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= Enumerable.Empty<NavLink>();
    }

    protected RenderFragment Render(NavLink item) => new RenderFragment(builder =>
    {
        var index = 0;
        builder.OpenComponent<NavLink>(index++);
        builder.AddMultipleAttributes(index++, item.AdditionalAttributes);
        builder.AddAttribute(index++, nameof(NavLink.ActiveClass), item.ActiveClass);
        builder.AddAttribute(index++, nameof(NavLink.Match), item.Match);
        builder.AddAttribute(index++, nameof(NavLink.ChildContent), item.ChildContent);
        builder.CloseComponent();
    });
}
