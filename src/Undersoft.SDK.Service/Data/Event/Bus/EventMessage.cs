using System;
using System.Text.Json;

namespace Undersoft.SDK.Service.Data.Event.Bus
{
    public class EventMessage : Event
    {
        public EventMessage(object eventData, Type eventType)
        {
            EventData = JsonSerializer.SerializeToUtf8Bytes(eventData);
            EventType = eventType.FullName;
        }
    }
}