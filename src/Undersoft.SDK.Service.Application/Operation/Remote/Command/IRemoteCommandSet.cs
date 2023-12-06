using FluentValidation.Results;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;
using Undersoft.SDK.Service.Application.Operation;


public interface IRemoteCommandSet<TModel> : IRemoteCommandSet where TModel : class, IDataObject
{
    public new IEnumerable<RemoteCommand<TModel>> Commands { get; }
}

public interface IRemoteCommandSet : IOperation
{
    public IEnumerable<IRemoteCommand> Commands { get; }

    public ValidationResult Result { get; set; }
}
