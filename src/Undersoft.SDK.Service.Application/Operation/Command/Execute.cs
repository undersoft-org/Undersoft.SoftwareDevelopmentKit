namespace Undersoft.SDK.Service.Application.Operation.Command;
using SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data;

public class Execute<TStore, TType, TInput> : ActionCommand<TInput>
    where TInput : class
    where TType : class
    where TStore : IDataServiceStore
{
    public Execute(DataActionKind publishPattern) : base(CommandMode.Invoke, publishPattern) { }

    public Execute(DataActionKind publishPattern, TInput input)
        : base(CommandMode.Invoke, publishPattern, input)  { }
}
