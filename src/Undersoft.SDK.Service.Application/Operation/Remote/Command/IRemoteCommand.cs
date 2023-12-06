using FluentValidation.Results;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

using Undersoft.SDK.Service.Data.Entity;


public interface IRemoteCommand : IOperation
{
    long Id { get; set; }

    object[] Keys { get; set; }

    IDataObject Contract { get; set; }

    object Model { get; set; }

    ValidationResult Result { get; set; }

    bool IsValid { get; }
}
