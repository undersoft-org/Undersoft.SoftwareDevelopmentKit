using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace Undersoft.SDK.Service.Application.Components;

public class DataTableDynamicContext : DynamicObjectContext
{
    public DataTable? DataTable { get; set; }

    private Type DynamicObjectType { get; }

    private IEnumerable<ITableColumn> Columns { get; }

    private List<IDynamicObject>? Items { get; set; }

    private Action<DataTableDynamicContext, ITableColumn>? AddAttributesCallback { get; set; }

    private ConcurrentDictionary<Guid, (IDynamicObject DynamicObject, DataRow Row)> Caches { get; } = new();

    public Func<IEnumerable<IDynamicObject>, Task>? OnAddAsync { get; set; }

    public Func<IEnumerable<IDynamicObject>, Task<bool>>? OnDeleteAsync { get; set; }

    public DataTableDynamicContext(DataTable table, Action<DataTableDynamicContext, ITableColumn>? addAttributesCallback = null, IEnumerable<string>? invisibleColumns = null, IEnumerable<string>? shownColumns = null, IEnumerable<string>? hiddenColumns = null)
    {
        DataTable = table;
        AddAttributesCallback = addAttributesCallback;

        var cols = InternalGetColumns();

        DynamicObjectType = CreateType();

        Columns = Utility.GetTableColumns(DynamicObjectType, cols).Where(col => GetShownColumns(col, invisibleColumns, shownColumns, hiddenColumns));

        OnValueChanged = OnCellValueChanged;

        [ExcludeFromCodeCoverage]
        Type CreateType()
        {
            var dynamicType = EmitHelper.CreateTypeByName($"BootstrapBlazor_{nameof(DataTableDynamicContext)}_{GetHashCode()}", cols, typeof(DataTableDynamicObject), OnColumnCreating);
            return dynamicType ?? throw new InvalidOperationException();
        }
    }

    private static bool GetShownColumns(ITableColumn col, IEnumerable<string>? invisibleColumns, IEnumerable<string>? shownColumns, IEnumerable<string>? hiddenColumns)
    {
        var ret = true;
        var columnName = col.GetFieldName();
        if (invisibleColumns != null && invisibleColumns.Any(c => c.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
        {
            ret = false;
        }

        if (ret && hiddenColumns != null && hiddenColumns.Any(c => c.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
        {
            col.Visible = false;
        }

        if (ret && shownColumns != null && !shownColumns.Any(c => c.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
        {
            col.Visible = true;
        }
        return ret;
    }

    public override IEnumerable<IDynamicObject> GetItems()
    {
        Items ??= BuildItems();
        return Items;
    }

    private List<IDynamicObject> BuildItems()
    {
        Caches.Clear();
        var ret = new List<IDynamicObject>();
        foreach (DataRow row in DataTable.Rows)
        {
            if (row.RowState != DataRowState.Deleted)
            {
                var dynamicObject = Activator.CreateInstance(DynamicObjectType);
                if (dynamicObject is DataTableDynamicObject d)
                {
                    foreach (DataColumn col in DataTable.Columns)
                    {
                        if (!row.IsNull(col))
                        {
                            Utility.SetPropertyValue<object, object?>(d, col.ColumnName, row[col]);
                        }
                    }

                    d.Row = row;
                    d.DynamicObjectPrimaryKey = Guid.NewGuid();
                    Caches.TryAdd(d.DynamicObjectPrimaryKey, (d, row));
                    ret.Add(d);
                }
            }
        }
        return ret;
    }

    public override IEnumerable<ITableColumn> GetColumns() => Columns;

    private IEnumerable<ITableColumn> InternalGetColumns()
    {
        var ret = new List<InternalTableColumn>();
        foreach (DataColumn col in DataTable.Columns)
        {
            ret.Add(new InternalTableColumn(col.ColumnName, col.DataType));
        }
        return ret;
    }

    protected internal override IEnumerable<CustomAttributeBuilder> OnColumnCreating(ITableColumn col)
    {
        AddAttributesCallback?.Invoke(this, col);
        return base.OnColumnCreating(col);
    }

    #region Add Save Delete
    public override async Task AddAsync(IEnumerable<IDynamicObject> selectedItems)
    {
        if (OnAddAsync != null)
        {
            await OnAddAsync(selectedItems);
        }
        else if (Activator.CreateInstance(DynamicObjectType) is DataTableDynamicObject dynamicObject)
        {
            var row = DataTable.NewRow();
            var indexOfRow = 0;
            var item = selectedItems.FirstOrDefault();

            if (item != null && Caches.TryGetValue(item.DynamicObjectPrimaryKey, out var c))
            {
                indexOfRow = DataTable.Rows.IndexOf(c.Row);
            }

            DataTable.Rows.InsertAt(row, indexOfRow);

            dynamicObject.DynamicObjectPrimaryKey = Guid.NewGuid();
            foreach (DataColumn col in DataTable.Columns)
            {
                if (col.DefaultValue != DBNull.Value)
                {
                    Utility.SetPropertyValue<object, object?>(dynamicObject, col.ColumnName, col.DefaultValue);
                }
            }
            dynamicObject.Row = row;

            if (OnChanged != null)
            {
                await OnChanged(new(new[] { dynamicObject }, DynamicItemChangedType.Add));
            }

            Items?.Insert(indexOfRow, dynamicObject);

            Caches.TryAdd(dynamicObject.DynamicObjectPrimaryKey, (dynamicObject, row));
        }
    }

    public override async Task<bool> DeleteAsync(IEnumerable<IDynamicObject> items)
    {
        var ret = false;
        if (OnDeleteAsync != null)
        {
            ret = await OnDeleteAsync(items);
            Items?.RemoveAll(i => items.Any(item => item == i));
        }
        else
        {
            var changed = false;
            foreach (var item in items)
            {
                if (Caches.TryGetValue(item.DynamicObjectPrimaryKey, out var row))
                {
                    changed = true;

                    DataTable.Rows.Remove(row.Row);

                    Caches.TryRemove(item.DynamicObjectPrimaryKey, out _);

                    Items?.Remove(item);
                }
            }
            if (changed)
            {
                DataTable.AcceptChanges();
                if (OnChanged != null)
                {
                    await OnChanged(new(items, DynamicItemChangedType.Delete));
                }
            }
            ret = true;
        }
        return ret;
    }

    private Task OnCellValueChanged(IDynamicObject item, ITableColumn column, object? val)
    {
        if (Caches.TryGetValue(item.DynamicObjectPrimaryKey, out var cacheItem))
        {
            cacheItem.Row[column.GetFieldName()] = val;
            Items = null;
        }
        return Task.CompletedTask;
    }
    #endregion
}
