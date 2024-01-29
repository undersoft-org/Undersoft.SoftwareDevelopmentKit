using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Query.Handler;

using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class FilterAsyncHandler<TStore, TEntity, TDto>
    : IStreamRequestHandler<FilterAsync<TStore, TEntity, TDto>, TDto>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;

    public FilterAsyncHandler(IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
    }

    public virtual IAsyncEnumerable<TDto> Handle(
        FilterAsync<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if (request.Predicate == null)
            return _repository.FilterAsync<TDto>(
                request.Offset,
                request.Limit,
                request.Sort,
                request.Expanders
            );
        return _repository.FilterAsync<TDto>(
            request.Offset,
            request.Limit,
            request.Predicate,
            request.Sort,
            request.Expanders
        );
    }
}
