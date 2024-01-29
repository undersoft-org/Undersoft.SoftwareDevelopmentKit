namespace Undersoft.SDK.Service.Application.Components;

public interface ITable
{
    List<ITableColumn> Columns { get; }

    IEnumerable<ITableColumn> GetVisibleColumns();

    Dictionary<string, IFilterAction> Filters { get; }

    Func<Task> OnFilterAsync { get; }
}
