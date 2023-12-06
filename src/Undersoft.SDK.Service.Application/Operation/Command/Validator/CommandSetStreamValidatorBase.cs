namespace Undersoft.SDK.Service.Application.Operation.Command.Validator;

public abstract class CommandSetStreamValidatorBase<TCommand> : CommandSetValidatorBase<TCommand> where TCommand : ICommandSet
{
    public CommandSetStreamValidatorBase(IServicer servicer) : base(servicer)
    {
    }
}