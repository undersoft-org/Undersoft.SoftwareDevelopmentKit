namespace Undersoft.SDK.Service.Application.Components;

public partial class Tooltip : ITooltip
{
    protected string? PlacementString => Placement == Placement.Auto ? null : Placement.ToDescriptionString();

    protected string? SanitizeString => Sanitize ? null : "false";

    protected string? HtmlString => IsHtml ? "true" : null;

    [NotNull]
    protected string? ToggleString { get; set; }

    protected string? ClassString => CssBuilder.Default()
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public string? Delay { get; set; }

    [Parameter]
    public string? Selector { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool IsHtml { get; set; }

    [Parameter]
    public bool Sanitize { get; set; } = true;

    [Parameter]
    public Placement Placement { get; set; } = Placement.Top;

    [Parameter]
    public string? CustomClass { get; set; }

    [Parameter]
    public string? Trigger { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected virtual string? CustomClassString => CustomClass;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToggleString = "tooltip";
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Trigger ??= "focus hover";
    }

    public void SetParameters(string title, Placement placement = Placement.Auto, string? trigger = null, string? customClass = null, bool? isHtml = null, bool? sanitize = null, string? delay = null, string? selector = null)
    {
        Title = title;
        if (placement != Placement.Auto) Placement = placement;
        if (!string.IsNullOrEmpty(trigger)) Trigger = trigger;
        if (!string.IsNullOrEmpty(customClass)) CustomClass = customClass;
        if (isHtml.HasValue) IsHtml = isHtml.Value;
        if (sanitize.HasValue) Sanitize = sanitize.Value;
        if (!string.IsNullOrEmpty(delay)) Delay = delay;
        if (!string.IsNullOrEmpty(selector)) Selector = selector;
        StateHasChanged();
    }
}
