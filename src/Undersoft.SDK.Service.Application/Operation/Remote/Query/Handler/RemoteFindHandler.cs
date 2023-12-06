using MediatR;


namespace Undersoft.SDK.Service.Application.Operation.Remote.Query.Handler;

public class RemoteFindHandler<TStore, TDto, TModel> : IRequestHandler<RemoteFind<TStore, TDto, TModel>, TModel>
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;

    public RemoteFindHandler(IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
    }

    public virtual Task<TModel> Handle(
        RemoteFind<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (request.Keys != null)
            return _repository.Find<TModel>(request.Keys, request.Expanders);
        return _repository.Find<TModel>(request.Predicate, false, request.Expanders);
    }
}
