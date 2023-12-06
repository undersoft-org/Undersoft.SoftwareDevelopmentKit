using System;
using System.Collections.Concurrent;
using Undersoft.SDK.Series;
using System.Threading.Tasks;
using Undersoft.SDK.Service.Data.Event.Bus;

namespace Undersoft.SDK.Service.Data.Event.Handler
{

    public class EventHandlerInvoker : IEventHandlerInvoker
    {
        private readonly Registry<IEventHandlerMethodExecutor> _cache;

        public EventHandlerInvoker()
        {
            _cache = new Registry<IEventHandlerMethodExecutor>();
        }

        public async Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType)
        {
            var cacheItem = _cache.EnsureGet($"{eventHandler.GetType().FullName}-{eventType.FullName}", _ =>
            {
                return (IEventHandlerMethodExecutor)typeof(EventHandlerMethodExecutor<>).MakeGenericType(eventType).New();
            });



            if (cacheItem.Value != null)
            {
                await cacheItem.Value.ExecutorAsync(eventHandler, eventData);
            }
            else
            {
                throw new Exception("The object instance is not an event handler. Structure type: " + eventHandler.GetType().AssemblyQualifiedName);
            }
        }
    }
}