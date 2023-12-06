using MediatR;
using Undersoft.SDK.Series;


namespace Undersoft.SDK.Service.Application.Operation.Remote.Query.Handler;

public class RemoteFilterHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteFilter<TStore, TDto, TModel>, ISeries<TModel>>
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;

    public RemoteFilterHandler(IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
    }

    public virtual Task<ISeries<TModel>> Handle(
        RemoteFilter<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (request.Predicate == null)
            return _repository.Filter<TModel>(
                request.Offset,
                request.Limit,
                request.Sort,
                request.Expanders
            );
        return _repository.Filter<TModel>(
            request.Offset,
            request.Limit,
            request.Predicate,
            request.Sort,
            request.Expanders
        );
    }
}
