using FluentValidation.Results;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command;

using Undersoft.SDK.Service.Data.Entity;


public interface IRemoteCommand : IOperation
{
    long Id { get; set; }

    object[] Keys { get; set; }

    IOrigin Contract { get; set; }

    object Model { get; set; }

    ValidationResult Result { get; set; }

    bool IsValid { get; }
}
