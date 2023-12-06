namespace Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Entity;

public interface IEvent : IEntity
{
    uint EventVersion { get; set; }
    string EventType { get; set; }
    byte[] EventData { get; set; }
    long AggregateId { get; set; }
    string AggregateType { get; set; }
    DateTime PublishTime { get; set; }
    EventPublishStatus PublishStatus { get; set; }
}