namespace Undersoft.SDK.Service.Application.Components;

public interface IValidator
{
    void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results);
}
