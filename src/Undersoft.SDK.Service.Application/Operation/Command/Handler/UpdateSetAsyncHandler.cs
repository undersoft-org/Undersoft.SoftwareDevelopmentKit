using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class UpdateSetAsyncHandler<TStore, TEntity, TDto>
    : IStreamRequestHandler<UpdateSetAsync<TStore, TEntity, TDto>, Command<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public UpdateSetAsyncHandler(IServicer servicer, IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public IAsyncEnumerable<Command<TDto>> Handle(
        UpdateSetAsync<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IAsyncEnumerable<TEntity> entities;
            if (request.Predicate == null)
                entities = _repository.SetByAsync(request.ForOnly(d => d.IsValid, d => d.Contract));
            else if (request.Conditions == null)
                entities = _repository.SetByAsync(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate
                );
            else
                entities = _repository.SetByAsync(
                    request.ForOnly(d => d.IsValid, d => d.Contract),
                    request.Predicate,
                    request.Conditions
                );

            var response = entities.ForEachAsync(
                (e) =>
                {
                    var r = request[e.Id];
                    r.Entity = e;
                    return r;
                }
            );

            _ = _servicer
                .Publish(new UpdatedSet<TStore, TEntity, TDto>(request))
                .ConfigureAwait(false);

            return response;
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return null;
    }
}
