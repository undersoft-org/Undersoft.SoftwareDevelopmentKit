using FluentValidation.Results;

namespace Undersoft.SDK.Service.Server.Operation.Command;


using Undersoft.SDK.Service.Data.Object;

public interface ICommandSet<TDto> : ICommandSet where TDto : class, IOrigin, IInnerProxy
{
    public new IEnumerable<Command<TDto>> Commands { get; }
}

public interface ICommandSet : IOperation
{
    public IEnumerable<ICommand> Commands { get; }

    public ValidationResult Result { get; set; }
}
