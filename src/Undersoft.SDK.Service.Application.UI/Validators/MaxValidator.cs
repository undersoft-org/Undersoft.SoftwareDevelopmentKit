using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

public class MaxValidator : ValidatorBase
{
    public string? ErrorMessage { get; set; }

    public int Value { get; set; }

    public Func<string, int> SplitCallback { get; set; } = value => value.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;

    protected virtual string GetErrorMessage() => ErrorMessage ?? "At most {0} items can be selected";

    public override void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results)
    {
        if (!Validate(propertyValue))
        {
            var errorMessage = string.Format(CultureInfo.CurrentCulture, GetErrorMessage(), Value);
            results.Add(new ValidationResult(errorMessage, new string[] { context.MemberName ?? context.DisplayName }));
        }
    }

    protected virtual bool Validate(object? propertyValue)
    {
        var ret = true;
        if (propertyValue != null)
        {
            var type = propertyValue.GetType();
            if (propertyValue is string value)
            {
                var count = SplitCallback(value);
                ret = Validate(count);
            }
            else if (type.IsGenericType || type.IsArray)
            {
                ret = Validate(LambdaExtensions.ElementCount(propertyValue));
            }
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    protected virtual bool Validate(int count) => count <= Value;
}
