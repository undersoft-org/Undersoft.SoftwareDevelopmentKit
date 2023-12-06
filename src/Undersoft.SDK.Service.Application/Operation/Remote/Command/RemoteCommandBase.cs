using FluentValidation.Results;
using Undersoft.SDK.Service.Application.Operation.Command;

using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public abstract class RemoteCommandBase : IRemoteCommand
{
    private IDataObject contract;

    public virtual long Id { get; set; }

    public object[] Keys { get; set; }

    [JsonIgnore]
    public virtual IDataObject Contract
    {
        get => contract;
        set
        {
            contract = value;
            if (Id == 0 && contract.Id != 0)
                Id = contract.Id;
        }
    }

    [JsonIgnore]
    public virtual object Model { get; set; }

    [JsonIgnore]
    public ValidationResult Result { get; set; }

    public string ErrorMessages => Result.ToString();

    public CommandMode CommandMode { get; set; }

    public EventPublishMode PublishMode { get; set; }

    public virtual object Input => Model;

    public virtual object Output => IsValid ? Id : ErrorMessages;

    public bool IsValid => Result.IsValid;

    protected RemoteCommandBase()
    {
        Result = new ValidationResult();
    }

    protected RemoteCommandBase(CommandMode commandMode, EventPublishMode publishMode) : this()
    {
        CommandMode = commandMode;
        PublishMode = publishMode;
    }

    protected RemoteCommandBase(object entryData, CommandMode commandMode, EventPublishMode publishMode)
        : this(commandMode, publishMode)
    {
        Model = entryData;
    }

    protected RemoteCommandBase(
        object entryData,
        CommandMode commandMode,
        EventPublishMode publishMode,
        params object[] keys
    ) : this(commandMode, publishMode, keys)
    {
        Model = entryData;
    }

    protected RemoteCommandBase(
        CommandMode commandMode,
        EventPublishMode publishMode,
        params object[] keys
    ) : this(commandMode, publishMode)
    {
        Keys = keys;
    }
}
