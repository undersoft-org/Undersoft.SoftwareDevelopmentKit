using MediatR;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Query.Handler;

public class RemoteGetQueryHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteGetQuery<TStore, TDto, TModel>, IQueryable<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
    where TModel : class
{
    protected readonly IRemoteRepository<TDto> _repository;

    public RemoteGetQueryHandler(IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IQueryable<TModel>> Handle(
        RemoteGetQuery<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        return await _repository.GetQueryAsync<TModel>(request.Sort, request.Expanders);       
    }
}
