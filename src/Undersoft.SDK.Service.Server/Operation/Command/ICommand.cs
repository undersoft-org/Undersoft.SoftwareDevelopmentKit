using FluentValidation.Results;

namespace Undersoft.SDK.Service.Server.Operation.Command;


using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Object;

public interface ICommand : IOperation
{
    long Id { get; set; }

    object[] Keys { get; set; }

    IOrigin Entity { get; set; }

    object Contract { get; set; }

    ValidationResult Result { get; set; }

    bool IsValid { get; }
}
