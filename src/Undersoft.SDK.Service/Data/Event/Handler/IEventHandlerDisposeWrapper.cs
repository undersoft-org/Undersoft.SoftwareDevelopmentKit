using System;
using Undersoft.SDK.Service.Data.Event.Bus;

namespace Undersoft.SDK.Service.Data.Event.Handler
{
    public interface IEventHandlerDisposeWrapper : IDisposable
    {
        IEventHandler EventHandler { get; }
    }
}
