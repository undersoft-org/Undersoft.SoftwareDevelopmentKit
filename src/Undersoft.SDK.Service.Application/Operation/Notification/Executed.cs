namespace Undersoft.SDK.Service.Application.Operation.Notification;

using Command;
using SDK.Service.Data.Store;
using Undersoft.SDK;

public class Executed<TStore, TType, TDto> : ActionNotification<ActionCommand<TDto>>
    where TType : class
    where TDto : class, IOrigin
    where TStore : IDataServiceStore
{
    public Executed(Execute<TStore, TType, TDto> command) : base(command)
    {
    }
}
