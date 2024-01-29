using MediatR;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Service.Server.Operation.Query.Handler;

using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class FindQueryHandler<TStore, TEntity, TDto>
    : IRequestHandler<FindQuery<TStore, TEntity, TDto>, IQueryable<TDto>>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
    where TDto : class, IOrigin, IInnerProxy
{
    protected readonly IStoreRepository<TEntity> _repository;

    public FindQueryHandler(IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
    }

    public async virtual Task<IQueryable<TDto>> Handle(
        FindQuery<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<TDto> result = null;
        if (request.Keys != null)
            result = _repository.FindOneAsync<TDto>(request.Keys, request.Expanders);
        else
            result = _repository.FindOneAsync<TDto>(request.Predicate, request.Expanders);

        return await ValueTask.FromResult(result);
    }
}
