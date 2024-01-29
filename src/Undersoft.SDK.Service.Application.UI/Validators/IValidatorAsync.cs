namespace Undersoft.SDK.Service.Application.Components;

public interface IValidatorAsync : IValidator
{
    Task ValidateAsync(object? propertyValue, ValidationContext context, List<ValidationResult> results);
}
