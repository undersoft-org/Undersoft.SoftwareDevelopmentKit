using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

public class RequiredValidator : ValidatorBase
{
    public string? ErrorMessage { get; set; }

    public bool AllowEmptyString { get; set; }

    public IStringLocalizerFactory? LocalizerFactory { get; set; }

    public JsonLocalizationOptions? Options { get; set; }

    public override void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results)
    {
        var errorMessage = GetLocalizerErrorMessage(context, LocalizerFactory, Options);
        var memberNames = string.IsNullOrEmpty(context.MemberName) ? null : new string[] { context.MemberName };
        if (propertyValue == null)
        {
            results.Add(new ValidationResult(errorMessage, memberNames));
        }
        else if (propertyValue is string val)
        {
            if (!AllowEmptyString && val == string.Empty)
            {
                results.Add(new ValidationResult(errorMessage, memberNames));
            }
        }
        else if (propertyValue is IEnumerable v)
        {
            var enumerator = v.GetEnumerator();
            var valid = enumerator.MoveNext();
            if (!valid)
            {
                results.Add(new ValidationResult(errorMessage, memberNames));
            }
        }
    }

    protected virtual string GetRuleKey() => GetType().Name.Split(".").Last().Replace("Validator", "");

    protected virtual string? GetLocalizerErrorMessage(ValidationContext context, IStringLocalizerFactory? localizerFactory = null, JsonLocalizationOptions? options = null)
    {
        var errorMesssage = ErrorMessage;
        if (!string.IsNullOrEmpty(context.MemberName) && !string.IsNullOrEmpty(errorMesssage))
        {
            var memberName = context.MemberName;

            if (localizerFactory != null)
            {
                var isResx = false;
                if (options is { ResourceManagerStringLocalizerType: not null })
                {
                    var localizer = localizerFactory.Create(options.ResourceManagerStringLocalizerType);
                    if (localizer.TryGetLocalizerString(errorMesssage, out var resx))
                    {
                        errorMesssage = resx;
                        isResx = true;
                    }
                }

                if (!isResx && localizerFactory.Create(context.ObjectType).TryGetLocalizerString($"{memberName}.{GetRuleKey()}", out var msg))
                {
                    errorMesssage = msg;
                }
            }

            if (!string.IsNullOrEmpty(errorMesssage))
            {
                var displayName = new FieldIdentifier(context.ObjectInstance, context.MemberName).GetDisplayName();
                errorMesssage = string.Format(CultureInfo.CurrentCulture, errorMesssage, displayName);
            }
        }
        return errorMesssage;
    }
}
