using Undersoft.SDK.Service.Server.Operation.Command;

namespace Undersoft.SDK.Service.Server.Operation.Invocation;

using Undersoft.SDK.Service.Data.Store;

public class Action<TStore, TService, TDto> : Invocation<TDto>
    where TDto : class
    where TService : class
    where TStore : IDataServerStore
{
    public Action() : base() { }

    public Action(string method) : base(CommandMode.Invoke, typeof(TService), method) { }

    public Action(string method, Arguments arguments)
     : base(CommandMode.Invoke, typeof(TService), method, arguments) { }

    public Action(string method, params object[] arguments)
    : base(CommandMode.Invoke, typeof(TService), method, arguments) { }

}
 