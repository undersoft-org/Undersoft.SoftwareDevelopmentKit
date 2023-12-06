using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Event.Bus
{
    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task HandleEventAsync(TEvent eventData);
    }

    public interface IEventHandler
    {

    }
}
