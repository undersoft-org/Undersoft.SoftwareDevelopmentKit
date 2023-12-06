using MediatR;
using Undersoft.SDK.Series;

namespace Undersoft.SDK.Service.Application.Operation.Query.Handler;


using Undersoft.SDK.Service.Data.Repository;

public class GetHandler<TStore, TEntity, TDto>
    : IRequestHandler<Get<TStore, TEntity, TDto>, ISeries<TDto>>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;

    public GetHandler(IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
    }

    public virtual Task<ISeries<TDto>> Handle(
        Get<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if(request.Predicate != null)
            return _repository.Filter<TDto>(
            request.Offset,
            request.Limit,
            request.Predicate,
            request.Sort,
            request.Expanders
        );
        return _repository.Get<TDto>(
            request.Offset,
            request.Limit,
            request.Sort,
            request.Expanders
        );
    }
}
