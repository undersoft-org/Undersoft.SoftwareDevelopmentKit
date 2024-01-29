namespace Undersoft.SDK.Service.Application.Components;

internal class TableExportContext<TItem> : ITableExportContext<TItem>
{
    private ITable Table { get; }

    public IEnumerable<ITableColumn> Columns => Table.Columns;

    public IEnumerable<ITableColumn> GetVisibleColumns() => Table.GetVisibleColumns();

    public IEnumerable<TItem> Rows { get; }

    public Task ExportAsync() => ExportCallbackAsync();

    public QueryPageOptions BuildQueryPageOptions() => OptionsBuilder();

    private Func<QueryPageOptions> OptionsBuilder { get; }

    private Func<Task> ExportCallbackAsync { get; }

    public TableExportContext(ITable table, IEnumerable<TItem> rows, Func<QueryPageOptions> optionsBuilder, Func<Task> exportAsync)
    {
        Table = table;
        Rows = rows;
        ExportCallbackAsync = exportAsync;
        OptionsBuilder = optionsBuilder;
    }
}
