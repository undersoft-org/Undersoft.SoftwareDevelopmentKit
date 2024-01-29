using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Invocation.Notification.Handler;

using Logging;
using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;
using Undersoft.SDK.Service.Server.Operation.Invocation.Notification;

public class ActionInvokedHandler<TStore, TType, TDto>
    : INotificationHandler<ActionInvoked<TStore, TType, TDto>>
    where TType : class
    where TDto : class, IOrigin
    where TStore : IDataStore
{
    protected readonly IStoreRepository<Event> _eventStore;

    public ActionInvokedHandler() { }

    public ActionInvokedHandler(
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        ActionInvoked<TStore, TType, TDto> request,
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
                            $"{$"{GetType().Name} "}{$"for invoke {typeof(TType).Name} unable add event"}"
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
