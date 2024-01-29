using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class BoolFilter
{
    private string Value { get; set; } = "";

    [NotNull]
    private IEnumerable<SelectedItem>? Items { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TableFilter>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = new SelectedItem[]
        {
            new SelectedItem("", Localizer["BoolFilter.AllText"].Value),
            new SelectedItem("true", Localizer["BoolFilter.TrueText"].Value),
            new SelectedItem("false", Localizer["BoolFilter.FalseText"].Value)
        };

        if (TableFilter != null)
        {
            TableFilter.ShowMoreButton = false;
        }
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
            filters.Add(new FilterKeyValueAction()
            {
                FieldKey = FieldKey,
                FieldValue = Value == "true",
                FilterAction = FilterAction.Equal
            });
        }
        return filters;
    }

    public override async Task SetFilterConditionsAsync(IEnumerable<FilterKeyValueAction> conditions)
    {
        if (conditions.Any())
        {
            var first = conditions.First();
            if (first.FieldValue is bool value)
            {
                Value = value ? "true" : "false";
            }
            else if (first.FieldValue is null)
            {
                Value = "";
            }
        }
        await base.SetFilterConditionsAsync(conditions);
    }
}
