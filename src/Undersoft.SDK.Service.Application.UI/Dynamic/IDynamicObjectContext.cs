namespace Undersoft.SDK.Service.Application.Components;

public interface IDynamicObjectContext
{
    IEnumerable<ITableColumn> GetColumns();

    IEnumerable<IDynamicObject> GetItems();

    Task AddAsync(IEnumerable<IDynamicObject> selectedItems);

    Task<bool> DeleteAsync(IEnumerable<IDynamicObject> items);

    Func<IDynamicObject, ITableColumn, object?, Task>? OnValueChanged { get; set; }

    Func<DynamicObjectContextArgs, Task>? OnChanged { get; set; }

    Func<IDynamicObject?, IDynamicObject?, bool>? EqualityComparer { get; set; }
}
