using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Application.Operation.Command;
using Undersoft.SDK.Service.Data;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public class RemoteExecute<TStore, TDto, TModel> : ActionCommand<TModel>
    where TModel : class, IOrigin
    where TDto : class, IOrigin
    where TStore : IDataServiceStore
{
    public RemoteExecute(DataActionKind kind, TModel input)
        : base(CommandMode.Create, kind, input)
    {
        input.AutoId();
    }
}
