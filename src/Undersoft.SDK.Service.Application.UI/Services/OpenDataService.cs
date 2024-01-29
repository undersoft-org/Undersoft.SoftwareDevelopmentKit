namespace Undersoft.SDK.Service.Application.Components;

using Undersoft.SDK.Service.Data.Remote;
using Undersoft.SDK.Service.Data.Store;
using Uniques;

public class OpenDataService<TModel> : DataServiceBase<TModel> where TModel : class, IUniqueIdentifiable, new()
{
    protected IRemoteSet<TModel> _dataSet;

    public OpenDataService(IRemoteSet<IDataStore, TModel> dataSet) { _dataSet = dataSet; }

    public override Task<bool> AddAsync(TModel model) => Task.FromResult(true);

    public override Task<bool> DeleteAsync(IEnumerable<TModel> models) => Task.FromResult(true);

    public override Task<bool> SaveAsync(TModel model, ItemChangedType changedType) => Task.FromResult(true);

    public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions option) => null;
}