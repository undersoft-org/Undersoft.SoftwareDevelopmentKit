namespace Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Object;

public class Event : DataObject, IEvent
{
    public virtual uint EventVersion { get; set; }
    public virtual string EventType { get; set; }
    public virtual long AggregateId { get; set; }
    public virtual string AggregateType { get; set; }
    public virtual byte[] EventData { get; set; }
    public virtual DateTime PublishTime { get; set; }
    public virtual EventPublishStatus PublishStatus { get; set; }
}