using MediatR;
using System.Text.Json;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Undersoft.SDK.Service.Data.Object;
using Remote.Command;

public abstract class RemoteNotification<TCommand> : Event, INotification where TCommand : RemoteCommandBase
{
    public TCommand Command { get; }

    protected RemoteNotification(TCommand command)
    {
        var aggregateTypeFullName = command.Contract.GetDataFullName();
        var eventTypeFullName = GetType().FullName;

        Command = command;
        Id = (long)Unique.NewId;
        AggregateId = command.Id;
        AggregateType = aggregateTypeFullName;
        EventType = eventTypeFullName;
        var dto = command.Contract;
        OriginKey = dto.OriginKey;
        OriginName = dto.OriginName;
        Modifier = dto.Modifier;
        Modified = dto.Modified;
        Creator = dto.Creator;
        Created = dto.Created;
        PublishStatus = EventPublishStatus.Ready;
        PublishTime = Log.Clock;

        EventData = JsonSerializer.SerializeToUtf8Bytes((RemoteCommandBase)command);
    }
}
