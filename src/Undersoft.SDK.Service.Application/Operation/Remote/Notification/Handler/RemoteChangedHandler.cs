using FluentValidation.Results;
using MediatR;



namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification.Handler;

public class RemoteChangedHandler<TStore, TDto, TCommand>
    : INotificationHandler<RemoteChanged<TStore, TDto, TCommand>>
    where TDto : class, IDataObject
    where TCommand : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IStoreRepository<Event> _eventStore;

    public RemoteChangedHandler() { }

    public RemoteChangedHandler(
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        RemoteChanged<TStore, TDto, TCommand> request,
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
                            $"{$"{GetType().Name} for entity "}{$"{typeof(TDto).Name} unable add event"}"
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
