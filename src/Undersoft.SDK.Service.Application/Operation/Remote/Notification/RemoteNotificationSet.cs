using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Command;
using Undersoft.SDK.Service.Data.Event;
using Series;

public abstract class RemoteNotificationSet<TCommand> : Catalog<RemoteNotification<TCommand>>, INotification
    where TCommand : RemoteCommandBase
{
    public EventPublishMode PublishMode { get; set; }

    protected RemoteNotificationSet(EventPublishMode publishPattern, RemoteNotification<TCommand>[] commands)
        : base(commands)
    {
        PublishMode = publishPattern;
    }
}
