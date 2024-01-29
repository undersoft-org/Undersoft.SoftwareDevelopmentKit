namespace Undersoft.SDK.Service.Application.Components;

public partial class Timeline
{
    protected string? ClassString => CssBuilder.Default("timeline")
        .AddClass("is-alternate", IsAlternate && !IsLeft)
        .AddClass("is-left", IsLeft)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    [NotNull]
    public IEnumerable<TimelineItem>? Items { get; set; }

    [Parameter]
    public bool IsReverse { get; set; }

    [Parameter]
    public bool IsAlternate { get; set; }

    [Parameter]
    public bool IsLeft { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= Enumerable.Empty<TimelineItem>();

        if (IsReverse)
        {
            var arr = Items.Reverse();
            Items = arr;
        }
    }
}
