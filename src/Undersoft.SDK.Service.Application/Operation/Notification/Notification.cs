using MediatR;
using System.Text.Json;

namespace Undersoft.SDK.Service.Application.Operation.Notification;

using Command;
using Logging;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Object;
using Uniques;

public abstract class Notification<TCommand> : Event, INotification where TCommand : CommandBase
{
    public TCommand Command { get; }

    protected Notification(TCommand command)
    {
        var aggregateTypeFullName = command.Entity.GetDataFullName();
        var eventTypeFullName = GetType().FullName;

        Command = command;
        Id = (long)Unique.NewId;
        AggregateId = command.Id;
        AggregateType = aggregateTypeFullName;
        EventType = eventTypeFullName;
        var entity = (Entity)command.Entity;
        OriginKey = entity.OriginKey;
        OriginName = entity.OriginName;
        Modifier = entity.Modifier;
        Modified = entity.Modified;
        Creator = entity.Creator;
        Created = entity.Created;
        PublishStatus = EventPublishStatus.Ready;
        PublishTime = Log.Clock;

        EventData = JsonSerializer.SerializeToUtf8Bytes((CommandBase)command);
    }
}
