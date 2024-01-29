namespace Undersoft.SDK.Service.Application.Components;

public partial class FilterButton<TValue>
{
    [Parameter]
    public Func<Task>? OnClearFilter { get; set; }

    [Parameter]
    public string? FilterIcon { get; set; }

    [Parameter]
    public string? ClearIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        FilterIcon ??= IconTheme.GetIconByKey(ComponentIcons.FilterButtonFilterIcon);
        ClearIcon ??= IconTheme.GetIconByKey(ComponentIcons.FilterButtonClearIcon);
    }

    private async Task ClearFilter()
    {
        if (OnClearFilter != null)
        {
            await OnClearFilter();
        }
    }
}
