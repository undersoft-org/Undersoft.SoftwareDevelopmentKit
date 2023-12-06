using FluentValidation.Results;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data.Entity;

public interface IActionCommand : IOperation
{
    long Id { get; set; }

    object[] Keys { get; set; }

    object Response { get; set; }

    object Data { get; set; }

    ValidationResult Result { get; set; }

    bool IsValid { get; }
}
