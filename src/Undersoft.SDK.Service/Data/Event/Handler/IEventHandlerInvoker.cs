using System;
using System.Threading.Tasks;
using Undersoft.SDK.Service.Data.Event.Bus;

namespace Undersoft.SDK.Service.Data.Event.Handler
{
    public interface IEventHandlerInvoker
    {
        Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType);
    }
}