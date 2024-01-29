using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Command;


using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Object;

public abstract class CommandBase : ICommand
{
    private IOrigin entity;

    public virtual long Id { get; set; }

    public object[] Keys { get; set; }

    public Delegate Processings {  get; set; }

    [JsonIgnore]
    public virtual IOrigin Entity
    {
        get => entity;
        set
        {
            entity = value;
            if (Id == 0 && entity.Id != 0)
                Id = entity.Id;
        }
    }

    [JsonIgnore]
    public virtual object Contract { get; set; }

    [JsonIgnore]
    public ValidationResult Result { get; set; }

    public string ErrorMessages => Result.ToString();

    public CommandMode CommandMode { get; set; }

    public EventPublishMode PublishMode { get; set; }

    public virtual object Input => Contract;

    public virtual object Output => IsValid ? Id : ErrorMessages;

    public bool IsValid => Result.IsValid;

    protected CommandBase()
    {
        Result = new ValidationResult();
    }

    protected CommandBase(CommandMode commandMode, EventPublishMode publishMode) : this()
    {
        CommandMode = commandMode;
        PublishMode = publishMode;
    }

    protected CommandBase(object entryData, CommandMode commandMode, EventPublishMode publishMode)
        : this(commandMode, publishMode)
    {
        Contract = entryData;
    }

    protected CommandBase(
        object entryData,
        CommandMode commandMode,
        EventPublishMode publishMode,
        params object[] keys
    ) : this(commandMode, publishMode, keys)
    {
        Contract = entryData;
    }

    protected CommandBase(
        CommandMode commandMode,
        EventPublishMode publishMode,
        params object[] keys
    ) : this(commandMode, publishMode)
    {
        Keys = keys;
    }
}
