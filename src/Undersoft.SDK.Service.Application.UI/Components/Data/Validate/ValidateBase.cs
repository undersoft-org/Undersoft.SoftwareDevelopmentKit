using Microsoft.AspNetCore.Components.Forms;
using Undersoft.SDK.Service.Application.Components;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class ValidateBase<TValue> : DisplayBase<TValue>, IValidateComponent
{
    private ValidationMessageStore? _parsingValidationMessages;

    protected bool PreviousParsingAttemptFailed { get; set; }

    protected string? PreviousErrorMessage { get; set; }

    protected EditContext? EditContext { get; set; }

    protected string? ErrorMessage { get; set; }

    protected string? ValidCss => IsValid.HasValue ? (IsValid.Value ? "is-valid" : "is-invalid") : null;

    protected bool? IsValid { get; set; }

    protected string? Disabled => IsDisabled ? "disabled" : null;

    protected string? Required { get; set; }

    protected TValue CurrentValue
    {
        get => Value;
        set
        {
            var hasChanged = !EqualityComparer<TValue>.Default.Equals(value, Value);
            if (hasChanged)
            {
                Value = value;

                if (FieldIdentifier != null)
                {
                    ValidateForm?.NotifyFieldChanged(FieldIdentifier.Value, Value);
                }
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(value);
                }
                if (OnValueChanged != null)
                {
                    _ = OnValueChanged.Invoke(value);
                }
                if (IsNeedValidate && FieldIdentifier != null)
                {
                    EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
                }
            }
        }
    }

    protected string CurrentValueAsString
    {
        get => FormatValueAsString(CurrentValue) ?? "";
        set
        {
            _parsingValidationMessages?.Clear();

            if (NullableUnderlyingType != null && string.IsNullOrEmpty(value))
            {
                PreviousParsingAttemptFailed = false;
                CurrentValue = default!;
            }
            else if (typeof(TValue) == typeof(object))
            {
                PreviousParsingAttemptFailed = false;
                CurrentValue = (TValue)(object)value;
            }
            else if (TryParseValueFromString(value, out var parsedValue, out var validationErrorMessage))
            {
                PreviousParsingAttemptFailed = false;
                CurrentValue = parsedValue;
            }
            else
            {
                PreviousParsingAttemptFailed = true;
                PreviousErrorMessage = validationErrorMessage;

                if (_parsingValidationMessages == null && EditContext != null)
                {
                    _parsingValidationMessages = new ValidationMessageStore(EditContext);
                }

                if (FieldIdentifier != null)
                {
                    _parsingValidationMessages?.Add(FieldIdentifier.Value, PreviousErrorMessage ?? "");

                    EditContext?.NotifyFieldChanged(FieldIdentifier.Value);
                }
            }

            if (PreviousParsingAttemptFailed)
            {
                EditContext?.NotifyValidationStateChanged();
            }
        }
    }

    [Parameter]
    public Func<TValue, Task>? OnValueChanged { get; set; }

    [Parameter]
    [NotNull]
    public string? ParsingErrorMessage { get; set; }

    [Parameter]
    public bool SkipValidate { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [CascadingParameter]
    protected EditContext? CascadedEditContext { get; set; }

    protected virtual bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage)
    {
        var ret = false;
        validationErrorMessage = null;
        if (value.TryConvertTo<TValue>(out result))
        {
            ret = true;
        }
        else
        {
            result = default;
            validationErrorMessage = FormatParsingErrorMessage();
        }
        return ret;
    }

    protected virtual string? FormatParsingErrorMessage() => ParsingErrorMessage;

    private bool IsRequired() => FieldIdentifier
        ?.Model.GetType().GetPropertyByName(FieldIdentifier.Value.FieldName)!.GetCustomAttribute<RequiredAttribute>(true) != null
        || (ValidateRules?.OfType<FormItemValidator>().Select(i => i.Validator).OfType<RequiredAttribute>().Any() ?? false);

    private string FieldClass => (EditContext != null && FieldIdentifier != null) ? EditContext.FieldCssClass(FieldIdentifier.Value) : "";

    protected string? CssClass => CssBuilder.Default()
        .AddClass(FieldClass, IsNeedValidate)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (EditContext == null)
        {
            if (CascadedEditContext != null)
            {
                EditContext = CascadedEditContext;
            }
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (ValidateForm != null && FieldIdentifier.HasValue)
        {
            ValidateForm.AddValidator((FieldIdentifier.Value.FieldName, ModelType: FieldIdentifier.Value.Model.GetType()), (FieldIdentifier.Value, this));
        }

        Id = (!string.IsNullOrEmpty(ValidateForm?.Id) && FieldIdentifier != null)
                ? $"{ValidateForm.Id}_{FieldIdentifier.Value.Model.GetHashCode()}_{FieldIdentifier.Value.FieldName}"
                : base.Id;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Required = (IsNeedValidate && !string.IsNullOrEmpty(DisplayText) && (ValidateForm?.ShowRequiredMark ?? false) && IsRequired()) ? "true" : null;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender && IsValid.HasValue)
        {
            var valid = IsValid.Value;
            if (valid)
            {
                await RemoveValidResult();
            }
            else
            {
                await ShowValidResult();
            }
        }
    }

    #region Validation
    protected List<IValidator> Rules { get; } = new();

    [Parameter]
    public List<IValidator>? ValidateRules { get; set; }

    public bool IsNeedValidate => !IsDisabled && !SkipValidate;

    public virtual bool IsComplexValue(object? propertyValue) => propertyValue != null
        && propertyValue is not string
        && !propertyValue.GetType().IsAssignableTo(typeof(System.Collections.IEnumerable))
        && propertyValue.GetType().IsClass;

    protected bool IsAsyncValidate { get; set; }

    public async Task ValidatePropertyAsync(object? propertyValue, ValidationContext context, List<ValidationResult> results)
    {
        if (IsNeedValidate)
        {
            ValidateType(context, results);

            if (results.Count == 0)
            {
                foreach (var validator in Rules)
                {
                    if (validator is IValidatorAsync v)
                    {
                        await v.ValidateAsync(propertyValue, context, results);
                        IsAsyncValidate = true;
                    }
                    else
                    {
                        validator.Validate(propertyValue, context, results);
                    }
                    if (results.Count > 0)
                    {
                        break;
                    }
                }
            }

            if (results.Count == 0 && ValidateRules != null)
            {
                foreach (var validator in ValidateRules)
                {
                    if (validator is IValidatorAsync v)
                    {
                        await v.ValidateAsync(propertyValue, context, results);
                        IsAsyncValidate = true;
                    }
                    else
                    {
                        validator.Validate(propertyValue, context, results);
                    }
                    if (results.Count > 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ValidateType(ValidationContext context, List<ValidationResult> results)
    {
        if (NullableUnderlyingType == null)
        {
            if (PreviousParsingAttemptFailed)
            {
                var memberNames = new string[] { context.MemberName! };
                results.Add(new ValidationResult(PreviousErrorMessage, memberNames));
            }
        }
    }

    public virtual void ToggleMessage(IEnumerable<ValidationResult> results, bool validProperty)
    {
        if (FieldIdentifier != null)
        {
            var messages = results.Where(item => item.MemberNames.Any(m => m == FieldIdentifier.Value.FieldName));
            if (messages.Any())
            {
                ErrorMessage = messages.First().ErrorMessage;
                IsValid = false;
            }
            else
            {
                ErrorMessage = null;
                IsValid = true;
            }

            OnValidate(IsValid);
        }

        if (IsAsyncValidate)
        {
            IsAsyncValidate = false;
            StateHasChanged();
        }
    }

    private JSModule? ValidateModule { get; set; }

    private Task<JSModule> LoadValidateModule() => JSRuntime.LoadModule2("validate");

    protected virtual async ValueTask ShowValidResult()
    {
        var id = RetrieveId();
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(ErrorMessage))
        {
            ValidateModule ??= await LoadValidateModule();
            await ValidateModule.InvokeVoidAsync("Validate.execute", id, ErrorMessage);
        }
    }

    protected virtual async ValueTask RemoveValidResult()
    {
        var id = RetrieveId();
        if (!string.IsNullOrEmpty(id))
        {
            ValidateModule ??= await LoadValidateModule();
            await ValidateModule.InvokeVoidAsync("Validate.dispose", id);
        }
    }

    protected virtual void OnValidate(bool? valid)
    {

    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (ValidateForm != null && FieldIdentifier.HasValue)
            {
                ValidateForm.TryRemoveValidator((FieldIdentifier.Value.FieldName, FieldIdentifier.Value.Model.GetType()), out _);
            }

            if (ValidateModule != null)
            {
                var id = RetrieveId();
                await ValidateModule.InvokeVoidAsync("Validate.dispose", id);
            }
        }

        await base.DisposeAsync(disposing);
    }
    #endregion

    public void SetDisable(bool disable)
    {
        IsDisabled = disable;
        StateHasChanged();
    }

    public void SetValue(TValue value)
    {
        CurrentValue = value;

        if (!ValueChanged.HasDelegate)
        {
            StateHasChanged();
        }
    }

    public void SetLabel(string label)
    {
        DisplayText = label;
        StateHasChanged();
    }
}
