namespace Undersoft.SDK.Service.Application.Operation.Command.Validator;



public class CommandSetStreamValidator<TDto> : CommandSetValidator<TDto> where TDto : class, IDataObject
{
    public CommandSetStreamValidator(IServicer servicer) : base(servicer) { }
}