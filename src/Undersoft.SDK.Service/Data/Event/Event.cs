namespace Undersoft.SDK.Service.Data.Event;

using System.Runtime.Serialization;
using Undersoft.SDK.Service.Data.Contract;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Model;
using Undersoft.SDK.Service.Data.Object;

[DataContract]
public class Event : DataObject, IEvent, IEntity, IContract
{
    public Event() : base() { }

    [DataMember(Order = 12)]
    public virtual uint EventVersion { get; set; }

    [DataMember(Order = 13)]
    public virtual string EventType { get; set; }

    [DataMember(Order = 14)]
    public virtual long AggregateId { get; set; }

    [DataMember(Order = 15)]
    public virtual string AggregateType { get; set; }

    [DataMember(Order = 16)]
    public virtual byte[] EventData { get; set; }

    [DataMember(Order = 17)]
    public virtual DateTime PublishTime { get; set; }

    [DataMember(Order = 18)]
    public virtual EventPublishStatus PublishStatus { get; set; }
}