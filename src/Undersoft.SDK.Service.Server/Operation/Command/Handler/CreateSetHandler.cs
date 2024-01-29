using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Handler;
using Logging;
using Notification;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class CreateSetHandler<TStore, TEntity, TDto>
    : IRequestHandler<CreateSet<TStore, TEntity, TDto>, CommandSet<TDto>>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public CreateSetHandler(IServicer uservice, IStoreRepository<TStore, TEntity> repository)
    {
        _servicer = uservice;
        _repository = repository;
    }

    public virtual async Task<CommandSet<TDto>> Handle(
        CreateSet<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TEntity> entities;
            if (request.Predicate == null)
                entities = _repository.AddBy(request.ForOnly(d => d.IsValid, d => d.Contract));
            else
                entities = _repository.AddBy(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate
                );

           await entities
                .ForEachAsync(
                    (e, x) =>
                    {
                        request[x].Entity = e;
                    }
                );               

            _ = _servicer
                .Publish(new CreatedSet<TStore, TEntity, TDto>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
