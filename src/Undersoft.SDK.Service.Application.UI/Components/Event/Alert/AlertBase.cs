namespace Undersoft.SDK.Service.Application.Components;

public abstract class AlertBase : Component
{
    protected virtual string? ClassName => CssBuilder.Default("alert fade show")
        .AddClass($"alert-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("alert-bar", ShowBar)
        .AddClass("alert-dismissible", ShowDismiss)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? IconString => CssBuilder.Default("alert-icon")
        .AddClass(Icon)
        .Build();

    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public bool ShowDismiss { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public bool ShowBar { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task>? OnDismiss { get; set; }
}
