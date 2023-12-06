using FluentValidation.Results;

namespace Undersoft.SDK.Service.Application.Operation.Command;


using Undersoft.SDK.Service.Data.Object;

public interface ICommandSet<TDto> : ICommandSet where TDto : class, IDataObject
{
    public new IEnumerable<Command<TDto>> Commands { get; }
}

public interface ICommandSet : IOperation
{
    public IEnumerable<ICommand> Commands { get; }

    public ValidationResult Result { get; set; }
}
