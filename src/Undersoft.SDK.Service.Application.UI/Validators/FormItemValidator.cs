namespace Undersoft.SDK.Service.Application.Components;

public class FormItemValidator : ValidatorBase
{
    public ValidationAttribute Validator { get; }

    public FormItemValidator(ValidationAttribute attribute)
    {
        Validator = attribute;
    }

    public override void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results)
    {
        var result = Validator.GetValidationResult(propertyValue, context);
        if (result != null)
        {
            results.Add(result);
        }
    }
}
