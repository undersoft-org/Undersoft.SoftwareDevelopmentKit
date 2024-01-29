using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Repeater<TItem>
{
    private string? RepeaterClassString => CssBuilder.Default("repeater")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    [Parameter]
    public bool ShowLoading { get; set; } = true;

    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    [Parameter]
    public bool ShowEmpty { get; set; } = true;

    [Parameter]
    public string? EmptyText { get; set; }

    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    [Parameter]
    public RenderFragment<RenderFragment>? ContainerTemplate { get; set; }

    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Repeater<TItem>>? Localizer { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        EmptyText ??= Localizer[nameof(EmptyText)];
    }

    private RenderFragment RenderItemTemplate(IEnumerable<TItem> items) => builder =>
    {
        if (ItemTemplate != null)
        {
            foreach (var item in items)
            {
                builder.AddContent(0, ItemTemplate(item));
            }
        }
    };
}
