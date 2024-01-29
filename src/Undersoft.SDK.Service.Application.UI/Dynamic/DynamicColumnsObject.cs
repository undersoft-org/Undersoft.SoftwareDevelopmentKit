namespace Undersoft.SDK.Service.Application.Components;

public class DynamicColumnsObject : IDynamicColumnsObject
{
    public Dictionary<string, object?> Columns { get; set; }

    public Guid DynamicObjectPrimaryKey { get; set; }

    public DynamicColumnsObject(Dictionary<string, object?> columnsData)
    {
        Columns = columnsData;
    }

    public DynamicColumnsObject() : this(new()) { }

    public virtual object? GetValue(string propertyName)
    {
        return Columns.TryGetValue(propertyName, out object? v) ? v : null;
    }

    public virtual void SetValue(string propertyName, object? value)
    {
        Columns[propertyName] = value;
    }
}
