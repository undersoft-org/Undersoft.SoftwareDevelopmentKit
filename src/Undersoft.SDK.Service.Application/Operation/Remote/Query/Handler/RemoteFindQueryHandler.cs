using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Query.Handler;

public class RemoteFindQueryHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteFindQuery<TStore, TDto, TModel>, IQueryable<TModel>>
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
    where TModel : class, IOrigin
{
    protected readonly IRemoteRepository<TDto> _repository;

    public RemoteFindQueryHandler(IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
    }

    public virtual async Task<IQueryable<TModel>> Handle(
        RemoteFindQuery<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        return await Task.Run(() =>
        {
            if (request.Keys != null)
                return _repository.FindOneAsync<TModel>(request.Keys, request.Expanders);
            else
                return _repository.FindOneAsync<TModel>(request.Predicate, request.Expanders);
        });
    }
}
