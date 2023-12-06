using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Notification;

using Command;
using Undersoft.SDK.Service.Data.Event;
using Series;

public abstract class NotificationSet<TCommand> : Catalog<Notification<TCommand>>, INotification
    where TCommand : CommandBase
{
    public EventPublishMode PublishMode { get; set; }

    protected NotificationSet(EventPublishMode publishPattern, Notification<TCommand>[] commands)
        : base(commands)
    {
        PublishMode = publishPattern;
    }
}
