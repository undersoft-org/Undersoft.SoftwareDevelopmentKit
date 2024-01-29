namespace Undersoft.SDK.Service.Application.Components;

public interface ITableExportContext<TItem>
{
    IEnumerable<ITableColumn> Columns { get; }

    IEnumerable<TItem> Rows { get; }

    QueryPageOptions BuildQueryPageOptions();

    Task ExportAsync();

    IEnumerable<ITableColumn> GetVisibleColumns();
}
