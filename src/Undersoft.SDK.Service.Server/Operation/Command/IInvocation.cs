using FluentValidation.Results;

namespace Undersoft.SDK.Service.Server.Operation.Command;

using Undersoft.SDK.Invoking;

public interface IInvocation : IOperation
{
    long Id { get; set; }

    Arguments Arguments { get; set; }

    string Method { get; set; }

    public Type ServiceType { get; set; }

    object Response { get; set; }

    ValidationResult Result { get; set; }

    bool IsValid { get; }
}
