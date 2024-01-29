using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class DynamicObjectContext : IDynamicObjectContext
{
    public abstract IEnumerable<ITableColumn> GetColumns();

    public abstract IEnumerable<IDynamicObject> GetItems();

    protected ConcurrentDictionary<string, List<CustomAttributeBuilder>> CustomerAttributeBuilderCache { get; } = new();

    public void AddAttribute(string columnName, Type attributeType, Type[] types, object?[] constructorArgs, PropertyInfo[]? propertyInfos = null, object?[]? propertyValues = null)
    {
        var attr = attributeType.GetConstructor(types);
        if (attr != null)
        {
            var cab = new CustomAttributeBuilder(attr, constructorArgs,
                namedProperties: propertyInfos ?? Array.Empty<PropertyInfo>(),
                propertyValues: propertyValues ?? Array.Empty<object?>());
            CustomerAttributeBuilderCache.AddOrUpdate(columnName,
                key => new List<CustomAttributeBuilder> { cab },
                (key, builders) =>
                {
                    builders.Add(cab);
                    return builders;
                });
        }
    }

    protected internal virtual IEnumerable<CustomAttributeBuilder> OnColumnCreating(ITableColumn col) => CustomerAttributeBuilderCache.TryGetValue(col.GetFieldName(), out var builders)
        ? builders
        : Enumerable.Empty<CustomAttributeBuilder>();

    public abstract Task AddAsync(IEnumerable<IDynamicObject> selectedItems);

    public abstract Task<bool> DeleteAsync(IEnumerable<IDynamicObject> items);

    public Func<IDynamicObject, ITableColumn, object?, Task>? OnValueChanged { get; set; }

    public Func<DynamicObjectContextArgs, Task>? OnChanged { get; set; }

    public Func<IDynamicObject?, IDynamicObject?, bool>? EqualityComparer { get; set; }
}
