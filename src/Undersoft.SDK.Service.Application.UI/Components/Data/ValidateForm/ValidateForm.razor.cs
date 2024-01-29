using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service.Application.Components;

public partial class ValidateForm
{
    [Parameter]
    [NotNull]
    public Func<EditContext, Task>? OnValidSubmit { get; set; }

    [Parameter]
    [NotNull]
    public Func<EditContext, Task>? OnInvalidSubmit { get; set; }

    [Parameter]
    [NotNull]
    public Action<string, object?>? OnFieldValueChanged { get; set; }

    [Parameter]
    public bool ValidateAllProperties { get; set; }

    [Parameter]
    [NotNull]
    public object? Model { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool ShowRequiredMark { get; set; } = true;

    [Parameter]
    public bool ShowLabel { get; set; } = true;

    [Parameter]
    public bool? ShowLabelTooltip { get; set; }

    [Parameter]
    public bool? DisableAutoSubmitFormByEnter { get; set; }

    [Inject]
    [NotNull]
    private IOptions<JsonLocalizationOptions>? Options { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<ApplicationOptions>? BootstrapBlazorOptions { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizerFactory? LocalizerFactory { get; set; }

    private ConcurrentDictionary<(string FieldName, Type ModelType), (FieldIdentifier FieldIdentifier, IValidateComponent ValidateComponent)> ValidatorCache { get; } = new();

    private string? DisableAutoSubmitString => (DisableAutoSubmitFormByEnter.HasValue && DisableAutoSubmitFormByEnter.Value) ? "true" : null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!DisableAutoSubmitFormByEnter.HasValue && BootstrapBlazorOptions.CurrentValue.DisableAutoSubmitFormByEnter.HasValue)
        {
            DisableAutoSubmitFormByEnter = BootstrapBlazorOptions.CurrentValue.DisableAutoSubmitFormByEnter.Value;
        }
    }

    internal void AddValidator((string FieldName, Type ModelType) key, (FieldIdentifier FieldIdentifier, IValidateComponent IValidateComponent) value)
    {
        ValidatorCache.TryAdd(key, value);
    }

    internal bool TryRemoveValidator((string FieldName, Type ModelType) key, [MaybeNullWhen(false)] out (FieldIdentifier FieldIdentifier, IValidateComponent IValidateComponent) value) => ValidatorCache.TryRemove(key, out value);

    public void SetError<TModel>(Expression<Func<TModel, object?>> expression, string errorMessage)
    {
        if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression mem)
        {
            InternalSetError(mem, errorMessage);
        }
        else if (expression.Body is MemberExpression exp)
        {
            InternalSetError(exp, errorMessage);
        }
    }

    private void InternalSetError(MemberExpression exp, string errorMessage)
    {
        var fieldName = exp.Member.Name;
        if (exp.Expression != null)
        {
            var modelType = exp.Expression.Type;
            var validator = ValidatorCache.FirstOrDefault(c => c.Key.ModelType == modelType && c.Key.FieldName == fieldName).Value.ValidateComponent;
            if (validator != null)
            {
                var results = new List<ValidationResult>
                {
                    new ValidationResult(errorMessage, new string[] { fieldName })
                };
                validator.ToggleMessage(results, true);
            }
        }
    }

    public void SetError(string propertyName, string errorMessage)
    {
        if (TryGetModelField(propertyName, out var modelType, out var fieldName) && TryGetValidator(modelType, fieldName, out var validator))
        {
            var results = new List<ValidationResult>
            {
                new ValidationResult(errorMessage, new string[] { fieldName })
            };
            validator.ToggleMessage(results, true);
        }
    }

    private bool TryGetModelField(string propertyName, [MaybeNullWhen(false)] out Type modelType, [MaybeNullWhen(false)] out string fieldName)
    {
        var propNames = new ConcurrentQueue<string>(propertyName.Split('.'));
        var modelTypeInfo = Model.GetType();
        modelType = null;
        fieldName = null;
        while (propNames.TryDequeue(out var propName))
        {
            modelType = modelTypeInfo;
            fieldName = propName;
            var propertyInfo = modelType.GetPropertyByName(propName);
            if (propertyInfo == null)
            {
                break;
            }
            var exp = Expression.Parameter(modelTypeInfo);
            var member = Expression.Property(exp, propertyInfo);
            modelTypeInfo = member.Type;
        }
        return propNames.IsEmpty;
    }

    private bool TryGetValidator(Type modelType, string fieldName, [NotNullWhen(true)] out IValidateComponent validator)
    {
        validator = ValidatorCache.FirstOrDefault(c => c.Key.ModelType == modelType && c.Key.FieldName == fieldName).Value.ValidateComponent;
        return validator != null;
    }

    private static bool IsPublic(PropertyInfo p) => p.GetMethod != null && p.SetMethod != null && p.GetMethod.IsPublic && p.SetMethod.IsPublic;

    internal async Task ValidateObject(ValidationContext context, List<ValidationResult> results)
    {
        if (ValidateAllProperties)
        {
            await ValidateProperty(context, results);
        }
        else
        {
            foreach (var key in ValidatorCache.Keys)
            {
                var validatorValue = ValidatorCache[key];
                var validator = validatorValue.ValidateComponent;
                var fieldIdentifier = validatorValue.FieldIdentifier;
                if (validator.IsNeedValidate)
                {
                    var messages = new List<ValidationResult>();
                    var pi = key.ModelType.GetPropertyByName(key.FieldName);
                    if (pi != null)
                    {
                        var propertyValidateContext = new ValidationContext(fieldIdentifier.Model, context, null)
                        {
                            MemberName = fieldIdentifier.FieldName,
                            DisplayName = fieldIdentifier.GetDisplayName()
                        };

                        var propertyValue = Utility.GetPropertyValue(fieldIdentifier.Model, fieldIdentifier.FieldName);

                        await ValidateAsync(validator, propertyValidateContext, messages, pi, propertyValue);
                    }
                    validator.ToggleMessage(messages, false);
                    results.AddRange(messages);
                }
            }
        }
    }

    internal async Task ValidateFieldAsync(ValidationContext context, List<ValidationResult> results)
    {
        if (!string.IsNullOrEmpty(context.MemberName) && ValidatorCache.TryGetValue((context.MemberName, context.ObjectType), out var v))
        {
            var validator = v.ValidateComponent;
            if (validator.IsNeedValidate)
            {
                var pi = context.ObjectType.GetPropertyByName(context.MemberName);
                if (pi != null)
                {
                    var propertyValue = Utility.GetPropertyValue(context.ObjectInstance, context.MemberName);
                    await ValidateAsync(validator, context, results, pi, propertyValue);
                }

                validator.ToggleMessage(results, true);
            }
        }
    }

    private void ValidateDataAnnotations(object? value, ValidationContext context, ICollection<ValidationResult> results, PropertyInfo propertyInfo, string? memberName = null)
    {
        var rules = propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>();
        var metadataType = context.ObjectType.GetCustomAttribute<MetadataTypeAttribute>(false);
        if (metadataType != null)
        {
            var p = metadataType.MetadataClassType.GetPropertyByName(propertyInfo.Name);
            if (p != null)
            {
                rules = rules.Concat(p.GetCustomAttributes(true).OfType<ValidationAttribute>());
            }
        }
        var displayName = context.DisplayName;
        memberName ??= propertyInfo.Name;
        var attributeSpan = nameof(Attribute).AsSpan();
        foreach (var rule in rules)
        {
            var result = rule.GetValidationResult(value, context);
            if (result != null && result != ValidationResult.Success)
            {
                var ruleNameSpan = rule.GetType().Name.AsSpan();
                var index = ruleNameSpan.IndexOf(attributeSpan, StringComparison.OrdinalIgnoreCase);
                var ruleName = ruleNameSpan[..index];
                var find = false;
                if (!string.IsNullOrEmpty(rule.ErrorMessage))
                {
                    var resxType = Options.Value.ResourceManagerStringLocalizerType;
                    if (resxType != null && LocalizerFactory.Create(resxType).TryGetLocalizerString(rule.ErrorMessage, out var resx))
                    {
                        rule.ErrorMessage = resx;
                        find = true;
                    }
                }

                if (!context.ObjectType.Assembly.IsDynamic && !find
                    && !string.IsNullOrEmpty(rule.ErrorMessage)
                    && LocalizerFactory.Create(context.ObjectType).TryGetLocalizerString(rule.ErrorMessage, out var msg))
                {
                    rule.ErrorMessage = msg;
                    find = true;
                }

                if (!rule.GetType().Assembly.IsDynamic && !find
                    && LocalizerFactory.Create(rule.GetType()).TryGetLocalizerString(nameof(rule.ErrorMessage), out msg))
                {
                    rule.ErrorMessage = msg;
                    find = true;
                }

                if (!context.ObjectType.Assembly.IsDynamic && !find
                    && LocalizerFactory.Create(context.ObjectType).TryGetLocalizerString($"{memberName}.{ruleName.ToString()}", out msg))
                {
                    rule.ErrorMessage = msg;
                    find = true;
                }

                if (!find)
                {
                    rule.ErrorMessage = result.ErrorMessage;
                }
                var errorMessage = !string.IsNullOrEmpty(rule.ErrorMessage) && rule.ErrorMessage.Contains("{0}")
                    ? rule.FormatErrorMessage(displayName)
                    : rule.ErrorMessage;
                results.Add(new ValidationResult(errorMessage, new string[] { memberName }));
            }
        }
    }

    private async Task ValidateProperty(ValidationContext context, List<ValidationResult> results)
    {
        var properties = context.ObjectType.GetRuntimeProperties().Where(p => IsPublic(p) && p.CanWrite && !p.GetIndexParameters().Any());
        foreach (var pi in properties)
        {
            var propertyValue = Utility.GetPropertyValue(context.ObjectInstance, pi.Name);
            var fieldIdentifier = new FieldIdentifier(context.ObjectInstance, pi.Name);
            context.DisplayName = fieldIdentifier.GetDisplayName();
            context.MemberName = fieldIdentifier.FieldName;

            if (ValidatorCache.TryGetValue((fieldIdentifier.FieldName, fieldIdentifier.Model.GetType()), out var v))
            {
                var validator = v.ValidateComponent;

                if (validator.IsComplexValue(propertyValue) && propertyValue != null)
                {
                    var fieldContext = new ValidationContext(propertyValue, context, null);
                    await ValidateProperty(fieldContext, results);
                }
                else
                {
                    var messages = new List<ValidationResult>();
                    if (validator.IsNeedValidate)
                    {
                        await ValidateAsync(validator, context, messages, pi, propertyValue);

                        validator.ToggleMessage(messages, true);
                    }
                    results.AddRange(messages);
                }
            }
        }
    }

    private async Task ValidateAsync(IValidateComponent validator, ValidationContext context, List<ValidationResult> messages, PropertyInfo pi, object? propertyValue)
    {
        if (validator is IUpload uploader)
        {
            if (uploader.UploadFiles.Count > 0)
            {
                uploader.UploadFiles.ForEach(file =>
                {
                    ValidateDataAnnotations(file.File, context, messages, pi, file.ValidateId);
                });
            }
            else
            {
                ValidateDataAnnotations(propertyValue, context, messages, pi);
            }
        }
        else
        {
            ValidateDataAnnotations(propertyValue, context, messages, pi);
            if (messages.Count == 0)
            {
                await validator.ValidatePropertyAsync(propertyValue, context, messages);
            }
        }

        _invalid = messages.Any();
    }

    private bool _invalid = false;

    private List<ButtonBase> AsyncSubmitButtons { get; } = new();

    internal void RegisterAsyncSubmitButton(ButtonBase button)
    {
        AsyncSubmitButtons.Add(button);
    }

    private async Task OnValidSubmitForm(EditContext context)
    {
        var isAsync = AsyncSubmitButtons.Any();
        foreach (var b in AsyncSubmitButtons)
        {
            b.TriggerAsync(true);
        }
        if (isAsync)
        {
            await Task.Yield();
        }
        if (OnValidSubmit != null)
        {
            await OnValidSubmit(context);
        }
        foreach (var b in AsyncSubmitButtons)
        {
            b.TriggerAsync(false);
        }
    }

    private async Task OnInvalidSubmitForm(EditContext context)
    {
        var isAsync = AsyncSubmitButtons.Any();
        foreach (var b in AsyncSubmitButtons)
        {
            b.TriggerAsync(true);
        }
        if (isAsync)
        {
            await Task.Yield();
        }
        if (OnInvalidSubmit != null)
        {
            await OnInvalidSubmit(context);
        }
        foreach (var b in AsyncSubmitButtons)
        {
            b.TriggerAsync(false);
        }
    }

    [NotNull]
    private PresenterDataAnnotationsValidator? Validator { get; set; }

    public bool Validate()
    {
        _invalid = true;
        var ret = Validator.Validate() && !_invalid;
        StateHasChanged();
        return ret;
    }

    public void NotifyFieldChanged(in FieldIdentifier fieldIdentifier, object? value)
    {
        ValueChagnedFields.AddOrUpdate(fieldIdentifier, key => value, (key, v) => value);
        OnFieldValueChanged?.Invoke(fieldIdentifier.FieldName, value);
    }

    public ConcurrentDictionary<FieldIdentifier, object?> ValueChagnedFields { get; } = new();
}
