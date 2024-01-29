namespace Undersoft.SDK.Service.Application.Components;

public interface IDataService<TModel> where TModel : class, new()
{
    Task<bool> AddAsync(TModel model);

    Task<bool> SaveAsync(TModel model, ItemChangedType changedType);

    Task<bool> DeleteAsync(IEnumerable<TModel> models);

    Task<QueryData<TModel>> QueryAsync(QueryPageOptions option);
}

internal class NullDataService<TModel> : DataServiceBase<TModel> where TModel : class, new()
{
    public override Task<QueryData<TModel>> QueryAsync(QueryPageOptions options) => Task.FromResult(new QueryData<TModel>()
    {
        Items = new List<TModel>(),
        TotalCount = 0
    });

    public override Task<bool> SaveAsync(TModel model, ItemChangedType changedType) => Task.FromResult(false);

    public override Task<bool> DeleteAsync(IEnumerable<TModel> models) => Task.FromResult(false);
}
