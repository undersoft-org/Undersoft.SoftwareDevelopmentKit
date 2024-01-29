namespace Undersoft.SDK.Service.Application.Components;

public class SearchFilterAction : IFilterAction
{
    public string Name { get; set; }

    public object? Value { get; set; }

    public FilterAction Action { get; set; }

    public SearchFilterAction(string name, object? value, FilterAction action = FilterAction.Contains)
    {
        Name = name;
        Value = value;
        Action = action;
    }

    public void Reset()
    {
        Value = null;
    }

    public Task SetFilterConditionsAsync(IEnumerable<FilterKeyValueAction> conditions)
    {
        if (conditions.Any())
        {
            var condition = conditions.FirstOrDefault(c => c.FieldKey == Name);
            if (condition != null)
            {
                Value = condition.FieldValue;
            }
        }
        return Task.CompletedTask;
    }

    public virtual IEnumerable<FilterKeyValueAction> GetFilterConditions() => new List<FilterKeyValueAction>()
    {
        new()
        {
            FieldKey = Name,
            FieldValue = Value,
            FilterAction = Action,
        }
    };
}
