using MediatR;
using Undersoft.SDK.Service.Application.Operation.Command;

using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public class RemoteCommand<TModel> : RemoteCommandBase, IRequest<RemoteCommand<TModel>>, IUnique where TModel : class, IDataObject
{
    [JsonIgnore]
    public override TModel Model => base.Model as TModel;

    protected RemoteCommand() { }

    protected RemoteCommand(CommandMode commandMode, TModel dataObject)
    {
        CommandMode = commandMode;
        base.Model = dataObject;
    }

    protected RemoteCommand(CommandMode commandMode, EventPublishMode publishMode, TModel dataObject)
        : base(dataObject, commandMode, publishMode) { }

    protected RemoteCommand(
        CommandMode commandMode,
        EventPublishMode publishMode,
        TModel dataObject,
        params object[] keys
    ) : base(dataObject, commandMode, publishMode, keys) { }

    protected RemoteCommand(CommandMode commandMode, EventPublishMode publishMode, params object[] keys)
        : base(commandMode, publishMode, keys) { }

    public byte[] GetBytes()
    {
        return Model.GetBytes();
    }

    public byte[] GetIdBytes()
    {
        return Model.GetIdBytes();
    }

    public bool Equals(IUnique other)
    {
        return Model.Equals(other);
    }

    public int CompareTo(IUnique other)
    {
        return Model.CompareTo(other);
    }

    public override long Id
    {
        get => Model.Id;
        set => Model.Id = value;
    }

    public long TypeId
    {
        get => Model.TypeId;
        set => Model.TypeId = value;
    }
}
