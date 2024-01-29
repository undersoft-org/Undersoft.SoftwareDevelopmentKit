namespace Undersoft.SDK.Service.Server.Operation.Command;



public interface ICommandSetStream<TDto> : ICommandSet where TDto : class, IOrigin, IInnerProxy
{
    public new IAsyncEnumerable<Command<TDto>> Commands { get; }
}
