using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Event.Handler;

namespace Undersoft.SDK.Service.Data.Event.Bus
{
    public class EventBus : EventBusBase, IEventBus
    {
        /// <summary>
        /// Reference to the Logger.
        /// </summary>
        public ILogger<EventBus> Logger { get; set; }

        protected EventBusOptions Options { get; }

        protected Registry<EventWithHandlerFactories> HandlerFactories { get; set; }

        public EventBus(
            IOptions<EventBusOptions> options,
            IServiceScopeFactory serviceScopeFactory,
            IEventHandlerInvoker eventHandlerInvoker)
            : base(serviceScopeFactory, eventHandlerInvoker)
        {
            Options = options.Value;
            Logger = NullLogger<EventBus>.Instance;

            HandlerFactories = new Registry<EventWithHandlerFactories>();
            SubscribeHandlers(Options.Handlers);
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }

        /// <inheritdoc/>
        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            var factories = GetOrCreateHandlerFactories(eventType, factory);

            if (!factory.IsInFactories(factories.EventHandlerFactories))
            {
                factories.EventHandlerFactories.Add(factory);
            }

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        /// <inheritdoc/>
        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            GetOrCreateHandlerFactories(typeof(TEvent)).EventHandlerFactories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as EventHandlerSingletonFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
        }

        /// <inheritdoc/>
        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType).EventHandlerFactories.RemoveAll(
                        factory =>
                            factory is EventHandlerSingletonFactory &&
                            (factory as EventHandlerSingletonFactory).HandlerInstance == handler
                    );
        }

        /// <inheritdoc/>
        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).EventHandlerFactories.Remove(factory);
        }

        /// <inheritdoc/>
        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).EventHandlerFactories.Clear();
        }

        protected override async Task PublishToEventBusAsync(Type eventType, object eventData)
        {
            await PublishAsync(new EventMessage(eventData, eventType));
        }

        public virtual async Task PublishAsync(EventMessage localEventMessage)
        {
            await TriggerHandlersAsync(Type.GetType(localEventMessage.EventType), localEventMessage.EventData);
        }

        protected override IEnumerable<EventWithHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventWithHandlerFactories>();

            foreach (var handlerFactory in HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.EventType)))
            {
                handlerFactoryList.Add(handlerFactory);
            }

            return handlerFactoryList;
        }

        private EventWithHandlerFactories GetOrCreateHandlerFactories(Type eventType, IEventHandlerFactory factory = null)
        {

            return HandlerFactories.EnsureGet(eventType, (type) => new EventWithHandlerFactories(eventType, factory != null ? new List<IEventHandlerFactory>() { factory } : null)).Value;
        }

        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }
    }
}
