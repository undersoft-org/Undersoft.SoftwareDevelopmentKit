using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class EnumFilter
{
    private string? Value { get; set; }

    private IEnumerable<SelectedItem> Items { get; set; } = Enumerable.Empty<SelectedItem>();

    [NotNull]
    private Type? EnumType { get; set; }

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

        if (Type == null) throw new InvalidOperationException("the Argument TypeName must be set.");

        if (TableFilter != null)
        {
            TableFilter.ShowMoreButton = false;
        }

        EnumType = Nullable.GetUnderlyingType(Type) ?? Type;
        Items = EnumType.ToSelectList(new SelectedItem("", Localizer["EnumFilter.AllText"].Value));
    }

    public override void Reset()
    {
        Value = "";
        StateHasChanged();
    }

    public override IEnumerable<FilterKeyValueAction> GetFilterConditions()
    {
        var filters = new List<FilterKeyValueAction>();
        if (!string.IsNullOrEmpty(Value) && Enum.TryParse(EnumType, Value, out var val))
        {
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
