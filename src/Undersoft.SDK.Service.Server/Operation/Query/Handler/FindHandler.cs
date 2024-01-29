using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Query.Handler;

using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class FindHandler<TStore, TEntity, TDto> : IRequestHandler<Find<TStore, TEntity, TDto>, TDto>
    where TDto : class
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;

    public FindHandler(IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
    }

    public virtual async Task<TDto> Handle(
        Find<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        return await Task.Run(() =>
        {
            if (request.Keys != null)
                return _repository.Find<TDto>(request.Keys, request.Expanders);
            return _repository.Find<TDto>(request.Predicate, false, request.Expanders);
        });
    }
}
