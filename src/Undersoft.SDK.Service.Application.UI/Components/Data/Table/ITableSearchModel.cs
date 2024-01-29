namespace Undersoft.SDK.Service.Application.Components;

public interface ITableSearchModel
{
    IEnumerable<IFilterAction> GetSearchs();

    void Reset();
}
