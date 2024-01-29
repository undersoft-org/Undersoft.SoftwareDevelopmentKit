namespace Undersoft.SDK.Service.Application.Components;

internal class DefaultExcelExport : ITableExcelExport
{
    public Task<bool> ExportAsync<TItem>(IEnumerable<TItem> items, IEnumerable<ITableColumn>? cols = null, string? fileName = null) where TItem : class
    {
        return Task.FromResult(false);
    }
}
