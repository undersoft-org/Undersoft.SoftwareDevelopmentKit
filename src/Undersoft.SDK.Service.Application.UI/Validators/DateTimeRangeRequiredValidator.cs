namespace Undersoft.SDK.Service.Application.Components;

public class DateTimeRangeRequiredValidator : RequiredValidator
{
    public override void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results)
    {
        if (propertyValue is DateTimeRangeValue d && (d.Start == DateTime.MinValue || d.End == DateTime.MinValue))
        {
            propertyValue = null;
        }
        var errorMessage = GetLocalizerErrorMessage(context, LocalizerFactory, Options);
        var memberNames = string.IsNullOrEmpty(context.MemberName) ? null : new string[] { context.MemberName };
        if (propertyValue == null)
        {
            results.Add(new ValidationResult(errorMessage, memberNames));
        }
    }
}
