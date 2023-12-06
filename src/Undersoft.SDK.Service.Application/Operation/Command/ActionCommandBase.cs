using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data;
using Undersoft.SDK.Service.Data.Entity;
using static Google.Protobuf.WellKnownTypes.Field.Types;

public abstract class ActionCommandBase : IActionCommand
{
    private object data;

    public virtual long Id { get; set; }

    public object[] Keys { get; set; }

    [JsonIgnore]
    public virtual object Response
    {
        get => data;
        set => data = value;
    }

    [JsonIgnore]
    public virtual object Data { get; set; }

    [JsonIgnore]
    public ValidationResult Result { get; set; }

    public string ErrorMessages => Result.ToString();

    public CommandMode CommandMode { get; set; }

    public DataActionKind Kind { get; set; }

    public virtual object Input => Data;

    public virtual object Output => IsValid ? Id : ErrorMessages;

    public bool IsValid => Result.IsValid;

    protected ActionCommandBase()
    {
        Result = new ValidationResult();
    }

    protected ActionCommandBase(CommandMode commandMode, DataActionKind kind) : this()
    {
        CommandMode = commandMode;
        Kind = kind;
    }

    protected ActionCommandBase(object entryData, CommandMode commandMode, DataActionKind publishMode)
        : this(commandMode, publishMode)
    {
        Data = entryData;
    }

    protected ActionCommandBase(
        object entryData,
        CommandMode commandMode,
    DataActionKind kind,
        params object[] keys
    ) : this(commandMode, kind, keys)
    {
        Data = entryData;
    }

    protected ActionCommandBase(
        CommandMode commandMode,
        DataActionKind kind,
        params object[] keys
    ) : this(commandMode, kind)
    {
        Keys = keys;
    }
}
