using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class BootstrapInputNumber<TValue>
{
    protected string? ButtonClassString => CssBuilder.Default("btn")
        .AddClass("btn-outline-secondary", Color == Color.None)
        .AddClass($"btn-outline-{Color.ToDescriptionString()}", Color != Color.None)
        .Build();

    protected string? InputClassString => CssBuilder.Default("form-control")
        .AddClass(CssClass).AddClass(ValidCss)
        .AddClass("Input-number-fix", ShowButton)
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [NotNull]
    private string? StepString { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnIncrement { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnDecrement { get; set; }

    [Parameter]
    public string? Min { get; set; }

    [Parameter]
    public string? Max { get; set; }

    [Parameter]
    public string? Step { get; set; }

    [Parameter]
    public bool ShowButton { get; set; }

    [Parameter]
    public string? MinusIcon { get; set; }

    [Parameter]
    public string? PlusIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<BootstrapInputNumber<TValue>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        if (!typeof(TValue).IsNumber())
        {
            throw new InvalidOperationException($"The type '{targetType}' is not a supported numeric type.");
        }

        return base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParsingErrorMessage ??= Localizer[nameof(ParsingErrorMessage)];
        MinusIcon ??= IconTheme.GetIconByKey(ComponentIcons.InputNumberMinusIcon);
        PlusIcon ??= IconTheme.GetIconByKey(ComponentIcons.InputNumberPlusIcon);

        SetStep();
    }

    protected override string? FormatParsingErrorMessage() => string.Format(CultureInfo.InvariantCulture, ParsingErrorMessage, DisplayText);

    protected override string? FormatValueAsString(TValue value) => Formatter != null
        ? Formatter.Invoke(value)
        : (!string.IsNullOrEmpty(FormatString) && value != null
            ? Utility.Format(value, FormatString)
            : InternalFormat(value));

    protected virtual string? InternalFormat(TValue value) => value switch
    {
        null => null,
        int @int => BindConverter.FormatValue(@int, CultureInfo.InvariantCulture),
        long @long => BindConverter.FormatValue(@long, CultureInfo.InvariantCulture),
        short @short => BindConverter.FormatValue(@short, CultureInfo.InvariantCulture),
        float @float => BindConverter.FormatValue(@float, CultureInfo.InvariantCulture),
        double @double => BindConverter.FormatValue(@double, CultureInfo.InvariantCulture),
        decimal @decimal => BindConverter.FormatValue(@decimal, CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException($"Unsupported type {value!.GetType()}"),
    };

    private void SetStep()
    {
        var val = CurrentValue;
        switch (val)
        {
            case int:
            case long:
            case short:
                StepString = Step ?? "1";
                break;
            case float:
            case double:
            case decimal:
                StepString = Step ?? "0.01";
                break;
        }
    }

    private async Task OnClickDec()
    {
        var val = CurrentValue;
        switch (val)
        {
            case int @int:
                val = (TValue)(object)(@int - int.Parse(StepString));
                break;
            case long @long:
                val = (TValue)(object)(@long - long.Parse(StepString));
                break;
            case short @short:
                val = (TValue)(object)(short)(@short - short.Parse(StepString));
                break;
            case float @float:
                val = (TValue)(object)(@float - float.Parse(StepString));
                break;
            case double @double:
                val = (TValue)(object)(@double - double.Parse(StepString));
                break;
            case decimal @decimal:
                val = (TValue)(object)(@decimal - decimal.Parse(StepString));
                break;
        }
        CurrentValue = SetMax(SetMin(val));
        if (OnDecrement != null)
        {
            await OnDecrement(CurrentValue);
        }
    }

    private async Task OnClickInc()
    {
        var val = CurrentValue;
        switch (val)
        {
            case int @int:
                val = (TValue)(object)(@int + int.Parse(StepString));
                break;
            case long @long:
                val = (TValue)(object)(@long + long.Parse(StepString));
                break;
            case short @short:
                val = (TValue)(object)(short)(@short + short.Parse(StepString));
                break;
            case float @float:
                val = (TValue)(object)(@float + float.Parse(StepString));
                break;
            case double @double:
                val = (TValue)(object)(@double + double.Parse(StepString));
                break;
            case decimal @decimal:
                val = (TValue)(object)(@decimal + decimal.Parse(StepString));
                break;
        }
        CurrentValue = SetMax(SetMin(val));
        if (OnIncrement != null)
        {
            await OnIncrement(CurrentValue);
        }
    }

    private void OnBlur()
    {
        if (!PreviousParsingAttemptFailed)
        {
            CurrentValue = SetMax(SetMin(Value));
        }
    }

    private TValue SetMin(TValue val)
    {
        if (!string.IsNullOrEmpty(Min))
        {
            switch (val)
            {
                case int @int:
                    val = (TValue)(object)Math.Max(@int, int.Parse(Min));
                    break;
                case long @long:
                    val = (TValue)(object)Math.Max(@long, long.Parse(Min));
                    break;
                case short @short:
                    val = (TValue)(object)Math.Max(@short, short.Parse(Min));
                    break;
                case float @float:
                    val = (TValue)(object)Math.Max(@float, float.Parse(Min));
                    break;
                case double @double:
                    val = (TValue)(object)Math.Max(@double, double.Parse(Min));
                    break;
                case decimal @decimal:
                    val = (TValue)(object)Math.Max(@decimal, decimal.Parse(Min));
                    break;
            }
        }
        return val;
    }

    private TValue SetMax(TValue val)
    {
        if (!string.IsNullOrEmpty(Max))
        {
            switch (val)
            {
                case int @int:
                    val = (TValue)(object)Math.Min(@int, int.Parse(Max));
                    break;
                case long @long:
                    val = (TValue)(object)Math.Min(@long, long.Parse(Max));
                    break;
                case short @short:
                    val = (TValue)(object)Math.Min(@short, short.Parse(Max));
                    break;
                case float @float:
                    val = (TValue)(object)Math.Min(@float, float.Parse(Max));
                    break;
                case double @double:
                    val = (TValue)(object)Math.Min(@double, double.Parse(Max));
                    break;
                case decimal @decimal:
                    val = (TValue)(object)Math.Min(@decimal, decimal.Parse(Max));
                    break;
            }
        }
        return val;
    }
}
