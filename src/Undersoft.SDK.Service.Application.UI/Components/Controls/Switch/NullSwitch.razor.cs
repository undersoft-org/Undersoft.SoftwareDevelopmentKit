using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class NullSwitch
{
    private string? ClassName => CssBuilder.Default("switch")
        .AddClass("is-checked", ComponentValue)
        .AddClass("disabled", IsDisabled)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? CoreClassName => CssBuilder.Default("switch-core")
        .AddClass($"border-{OnColor.ToDescriptionString()}", OnColor != Color.None && ComponentValue)
        .AddClass($"bg-{OnColor.ToDescriptionString()}", OnColor != Color.None && ComponentValue)
        .AddClass($"border-{OffColor.ToDescriptionString()}", OffColor != Color.None && !ComponentValue)
        .AddClass($"bg-{OffColor.ToDescriptionString()}", OffColor != Color.None && !ComponentValue)
        .Build();

    private string? GetInnerText()
    {
        string? ret = null;
        if (ShowInnerText)
        {
            ret = ComponentValue ? OnInnerText : OffInnerText;
        }
        return ret;
    }

    private string? Text => ComponentValue ? OnText : OffText;

    private string? SwitchStyleName => CssBuilder.Default()
        .AddClass($"min-width: {Width}px;", Width > 0)
        .AddStyleFromAttributes(AdditionalAttributes)
        .Build();

    protected override string? StyleName => CssBuilder.Default()
        .AddClass($"width: {Width}px;", Width > 0)
        .AddClass($"height: {Height}px;", Height >= 20)
        .Build();

    [Parameter]
    public Color OnColor { get; set; } = Color.Success;

    [Parameter]
    public Color OffColor { get; set; }

    [Parameter]
    public override int Width { get; set; } = 40;

    [Parameter]
    public int Height { get; set; } = 20;

    [Parameter]
    [NotNull]
    public string? OnInnerText { get; set; }

    [Parameter]
    [NotNull]
    public string? OffInnerText { get; set; }

    [Parameter]
    public bool ShowInnerText { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Switch>? Localizer { get; set; }

    [Parameter]
    public bool DefaultValueWhenNull { get; set; }

    protected bool ComponentValue => Value ?? DefaultValueWhenNull;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnInnerText ??= Localizer[nameof(OnInnerText)];
        OffInnerText ??= Localizer[nameof(OffInnerText)];

        if (CurrentValue == null)
        {
            CurrentValue = DefaultValueWhenNull;
        }
    }

    private async Task OnClick()
    {
        if (!IsDisabled)
        {
            Value = !ComponentValue;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            OnValueChanged?.Invoke(Value);
        }
    }
}
