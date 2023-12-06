using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class UpdateSetHandler<TStore, TEntity, TDto>
    : IRequestHandler<UpdateSet<TStore, TEntity, TDto>, CommandSet<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public UpdateSetHandler(IServicer servicer, IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<CommandSet<TDto>> Handle(
        UpdateSet<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TEntity> entities = null;
            if (request.Predicate == null)
                entities = _repository.SetBy(request.ForOnly(d => d.IsValid, d => d.Contract));
            else if (request.Conditions == null)
                entities = _repository.SetBy(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate
                );
            else
                entities = _repository.SetBy(
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

            _ = _servicer
                .Publish(new UpdatedSet<TStore, TEntity, TDto>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
