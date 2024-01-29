namespace Undersoft.SDK.Service.Application.Components;

public abstract class FilterBase : ComponentBase, IFilterAction
{
    protected string? FilterRowClassString => CssBuilder.Default("filter-row")
        .AddClass("active", HasFilter)
        .Build();

    protected virtual FilterLogic Logic { get; set; }

    protected string? FieldKey { get; set; }

    protected bool IsHeaderRow => TableFilter?.IsHeaderRow ?? false;

    protected bool HasFilter => TableFilter?.HasFilter ?? false;     

    [Parameter]
    public int Count { get; set; }

    [CascadingParameter]
    protected TableFilter? TableFilter { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (TableFilter != null)
        {
            TableFilter.FilterAction = this;
            FieldKey = TableFilter.FieldKey;
        }
    }

    public abstract void Reset();

    public abstract IEnumerable<FilterKeyValueAction> GetFilterConditions();

    public virtual Task SetFilterConditionsAsync(IEnumerable<FilterKeyValueAction> conditions) => OnFilterValueChanged();

    protected async Task OnFilterValueChanged()
    {
        if (TableFilter != null)
        {
            await TableFilter.OnFilterAsync();
            StateHasChanged();
        }
    }

    protected async Task OnClearFilter()
    {
        if (TableFilter != null)
        {
            Reset();
            await TableFilter.OnFilterAsync();
        }
    }
}
