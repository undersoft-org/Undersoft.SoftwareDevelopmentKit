using FluentValidation.Results;
using MediatR;



namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification.Handler;

public class RemoteUpdatedHandler<TStore, TDto, TModel>
    : INotificationHandler<RemoteUpdated<TStore, TDto, TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IStoreRepository<Event> _eventStore;

    public RemoteUpdatedHandler() { }

    public RemoteUpdatedHandler(
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        RemoteUpdated<TStore, TDto, TModel> request,
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
                            $"{$"{GetType().Name} or entity "}{$"{typeof(TDto).Name} unable add event"}"
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
