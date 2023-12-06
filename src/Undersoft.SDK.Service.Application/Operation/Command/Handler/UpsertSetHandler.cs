using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class UpsertSetHandler<TStore, TEntity, TDto>
    : IRequestHandler<UpsertSet<TStore, TEntity, TDto>, CommandSet<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _uservice;

    public UpsertSetHandler(IServicer uservice, IStoreRepository<TStore, TEntity> repository)
    {
        _uservice = uservice;
        _repository = repository;
    }

    public virtual async Task<CommandSet<TDto>> Handle(
        UpsertSet<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TEntity> entities;
            if (request.Conditions == null)
                entities = _repository.PutBy(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate
                );
            else
                entities = _repository.PutBy(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate,
                    request.Conditions
                );

            await entities
                .ForEachAsync(
                    (e) =>
                    {
                        request[e.Id].Entity = e;
                    }
                )
                .ConfigureAwait(false);

            _ = _uservice
                .Publish(new UpsertedSet<TStore, TEntity, TDto>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
