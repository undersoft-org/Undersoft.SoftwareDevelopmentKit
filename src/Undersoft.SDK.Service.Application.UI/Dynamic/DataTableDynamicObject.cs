using System.Data;

namespace Undersoft.SDK.Service.Application.Components;

public class DataTableDynamicObject : DynamicObject
{
    internal DataRow? Row { get; set; }

    public override object? GetValue(string propertyName)
    {
        object? ret = null;
        if (Row != null && Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached && Row.Table.Columns.Contains(propertyName))
        {
            if (Row.RowState == DataRowState.Added)
            {
                if (!Row.Table.Columns[propertyName]!.AutoIncrement)
                {
                    Row[propertyName] = base.GetValue(propertyName);
                }
            }
            ret = Row[propertyName];
        }
        return ret ?? base.GetValue(propertyName);
    }

    public override void SetValue(string propertyName, object? value)
    {
        base.SetValue(propertyName, value);

        if (Row != null && Row.Table.Columns.Contains(propertyName))
        {
            Row[propertyName] = value;
        }
    }
}
