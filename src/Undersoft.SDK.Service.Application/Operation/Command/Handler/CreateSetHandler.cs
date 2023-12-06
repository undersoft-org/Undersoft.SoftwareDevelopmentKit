using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class CreateSetHandler<TStore, TEntity, TDto>
    : IRequestHandler<CreateSet<TStore, TEntity, TDto>, CommandSet<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _uservice;

    public CreateSetHandler(IServicer uservice, IStoreRepository<TStore, TEntity> repository)
    {
        _uservice = uservice;
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
                entities = await _repository.AddBy(request.ForOnly(d => d.IsValid, d => d.Contract));
            else
                entities = await _repository.AddBy(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate
                );

            await entities
                .ForEachAsync(
                    (e, x) =>
                    {
                        request[x].Entity = e;
                    }
                )
                .ConfigureAwait(false);

            _ = _uservice
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
