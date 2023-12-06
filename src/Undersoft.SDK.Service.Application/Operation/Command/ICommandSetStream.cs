namespace Undersoft.SDK.Service.Application.Operation.Command;



public interface ICommandSetStream<TDto> : ICommandSet where TDto : class, IDataObject
{
    public new IAsyncEnumerable<Command<TDto>> Commands { get; }
}
