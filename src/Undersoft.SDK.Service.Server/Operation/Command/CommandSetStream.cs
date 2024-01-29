using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command;

using Undersoft.SDK.Service.Data.Event;

public class CommandSetStream<TDto>
    : CommandSet<TDto>,
        IStreamRequest<Command<TDto>>,
        ICommandSet<TDto> where TDto : class, IOrigin, IInnerProxy
{
    protected CommandSetStream() : base() { }

    protected CommandSetStream(CommandMode commandMode) : base(commandMode) { }

    protected CommandSetStream(
        CommandMode commandMode,
        EventPublishMode publishPattern,
        Command<TDto>[] DtoCommands
    ) : base(commandMode, publishPattern, DtoCommands) { }
}
