using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Notification.Handler;
using Logging;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;
using Undersoft.SDK.Service.Server.Operation.Command.Notification;

public class ChangedHandler<TStore, TEntity, TCommand>
    : INotificationHandler<Changed<TStore, TEntity, TCommand>>
    where TEntity : class, IOrigin, IInnerProxy
    where TCommand : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<Event> _eventStore;
    protected readonly IStoreRepository<TEntity> _repository;

    public ChangedHandler() { }

    public ChangedHandler(
        IStoreRepository<IReportStore, TEntity> repository,
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _repository = repository;
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        Changed<TStore, TEntity, TCommand> request,
        CancellationToken cancellationToken
    )
    {
        return Task.Run(
            async () =>
            {
                try
                {
                    if (_eventStore.Add(request) == null)
                        throw new Exception(
                            $"{$"{GetType().Name} for entity "}{$"{typeof(TEntity).Name} unable add event"}"
                        );

                    if (request.Command.PublishMode == EventPublishMode.PropagateCommand)
                    {
                        TEntity entity;
                        if (request.Command.Keys != null)
                            entity = await _repository.PatchBy(
                                request.Command.Contract,
                                request.Command.Keys
                            );
                        else
                            entity = await _repository.PatchBy(
                                request.Command.Contract,
                                request.Predicate
                            );

                        if (entity == null)
                            throw new Exception(
                                $"{$"{GetType().Name} for entity "}{$"{typeof(TEntity).Name} unable change report"}"
                            );

                        request.PublishStatus = EventPublishStatus.Complete;
                    }
                }
                catch (Exception ex)
                {
                    request.Command.Result.Errors.Add(
                        new ValidationFailure(string.Empty, ex.Message)
                    );
                    this.Failure<Domainlog>(ex.Message, request.Command.ErrorMessages, ex);
                    request.PublishStatus = EventPublishStatus.Error;
                }
            },
            cancellationToken
        );
    }
}
