namespace Undersoft.SDK.Service.Application.Components;

public class FilterKeyValueAction
{
    public string? FieldKey { get; set; }

    public object? FieldValue { get; set; }

    public FilterLogic FilterLogic { get; set; }

    public FilterAction FilterAction { get; set; }
}
