using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Query.Handler;


using Undersoft.SDK.Service.Data.Repository;

public class FindHandler<TStore, TEntity, TDto> : IRequestHandler<Find<TStore, TEntity, TDto>, TDto>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;

    public FindHandler(IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
    }

    public virtual Task<TDto> Handle(
        Find<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if (request.Keys != null)
            return _repository.Find<TDto>(request.Keys, request.Expanders);
        return _repository.Find<TDto>(request.Predicate, false, request.Expanders);
    }
}
