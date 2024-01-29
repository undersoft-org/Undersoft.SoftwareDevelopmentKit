using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class LookupFilter
{
    private string? Value { get; set; }

    private List<SelectedItem> Items { get; } = new List<SelectedItem>();

#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    [Parameter]
    [NotNull]
    public IEnumerable<SelectedItem>? Lookup { get; set; }

    [Parameter]
    public StringComparison LookupStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    [Parameter]
    [NotNull]
    public Type? Type { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TableFilter>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Lookup == null)
        {
            throw new InvalidOperationException("the Argument Lookup must be set.");
        }

        if (Type == null)
        {
            throw new InvalidOperationException("the Argument TypeName must be set.");
        }

        if (TableFilter != null)
        {
            TableFilter.ShowMoreButton = false;
        }
        Items.Add(new SelectedItem("", Localizer["EnumFilter.AllText"].Value));
        Items.AddRange(Lookup);
    }

    public override void Reset()
    {
        Value = "";
        StateHasChanged();
    }

    public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
    {
        var filters = new List<FilterKeyValueAction>();
        if (!string.IsNullOrEmpty(Value))
        {
            var type = Nullable.GetUnderlyingType(Type) ?? Type;
            var val = Convert.ChangeType(Value, type);
            filters.Add(new FilterKeyValueAction()
            {
                FieldKey = FieldKey,
                FieldValue = val,
                FilterAction = FilterAction.Equal
            });
        }
        return filters;
    }

    public override async Task SetFilterConditionsAsync(IEnumerable<FilterKeyValueAction> conditions)
    {
        if (conditions.Any())
        {
            var type = Nullable.GetUnderlyingType(Type) ?? Type;
            FilterKeyValueAction first = conditions.First();
            if (first.FieldValue != null && first.FieldValue.GetType() == type)
            {
                Value = first.FieldValue.ToString();
            }
            else
            {
                Value = "";
            }
        }
        await base.SetFilterConditionsAsync(conditions);
    }
}
