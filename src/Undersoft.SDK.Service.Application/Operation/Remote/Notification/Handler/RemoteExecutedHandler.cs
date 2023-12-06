using FluentValidation.Results;
using MediatR;
using Undersoft.SDK.Service.Application.Operation.Remote.Command;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification.Handler;

public class RemoteExecutedHandler<TStore, TDto, TModel>
    : INotificationHandler<RemoteExecuted<TStore, TDto, TModel>>
    where TDto : class, IOrigin
    where TModel : class, IOrigin
    where TStore : IDataServiceStore
{
    protected readonly IStoreRepository<Event> _eventStore;

    public RemoteExecutedHandler() { }

    public RemoteExecutedHandler(
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        RemoteExecuted<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        return Task.Run(
            () =>
            {
                try
                {
                    if (_eventStore.Add(request) == null)
                        throw new Exception(
                            $"{$"{GetType().Name} "}{$"for contract {typeof(TModel).Name} unable add event"}"
                        );
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
