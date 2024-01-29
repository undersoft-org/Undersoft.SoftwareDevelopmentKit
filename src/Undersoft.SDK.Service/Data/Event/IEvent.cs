namespace Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Contract;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Model;

public interface IEvent
{
    uint EventVersion { get; set; }
    string EventType { get; set; }
    byte[] EventData { get; set; }
    long AggregateId { get; set; }
    string AggregateType { get; set; }
    DateTime PublishTime { get; set; }
    EventPublishStatus PublishStatus { get; set; }
}