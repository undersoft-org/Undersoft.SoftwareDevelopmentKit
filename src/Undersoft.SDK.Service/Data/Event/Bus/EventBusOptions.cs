
using System.Collections.Generic;

namespace Undersoft.SDK.Service.Data.Event.Bus
{
    public class EventBusOptions
    {
        public IList<IEventHandler> Handlers { get; }

        public EventBusOptions()
        {
            Handlers = new List<IEventHandler>();
        }
    }
}
