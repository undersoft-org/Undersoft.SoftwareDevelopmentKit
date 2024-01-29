using Undersoft.SDK.Service.Server.Operation.Command;

namespace Undersoft.SDK.Service.Server.Operation.Invocation;

using Undersoft.SDK.Service.Data.Store;

public class Setup<TStore, TService, TDto> : Invocation<TDto>
    where TDto : class
    where TService : class
    where TStore : IDataServerStore
{
    public Setup() : base() { }

    public Setup(string method) : base(CommandMode.Invoke, typeof(TService), method) { }

    public Setup(string method, Arguments arguments)
     : base(CommandMode.Invoke, typeof(TService), method, arguments) { }

    public Setup(string method, params object[] arguments)
    : base(CommandMode.Invoke, typeof(TService), method, arguments) { }

}
 