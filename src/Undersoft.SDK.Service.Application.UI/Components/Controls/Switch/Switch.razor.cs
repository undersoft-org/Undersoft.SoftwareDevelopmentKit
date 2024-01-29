using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Switch
{
    private string? ClassName => CssBuilder.Default("switch")
        .AddClass("is-checked", Value)
        .AddClass("disabled", IsDisabled)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? CoreClassName => CssBuilder.Default("switch-core")
        .AddClass($"border-{OnColor.ToDescriptionString()}", OnColor != Color.None && Value)
        .AddClass($"bg-{OnColor.ToDescriptionString()}", OnColor != Color.None && Value)
        .AddClass($"border-{OffColor.ToDescriptionString()}", OffColor != Color.None && !Value)
        .AddClass($"bg-{OffColor.ToDescriptionString()}", OffColor != Color.None && !Value)
        .Build();

    private string? GetInnerText()
    {
        string? ret = null;
        if (ShowInnerText)
        {
            ret = Value ? OnInnerText : OffInnerText;
        }

        return ret;
    }

    private string? Text => Value ? OnText : OffText;

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

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnInnerText ??= Localizer[nameof(OnInnerText)];
        OffInnerText ??= Localizer[nameof(OffInnerText)];
    }

    private async Task OnClick()
    {
        if (!IsDisabled)
        {
            Value = !Value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (OnValueChanged != null)
            {
                await OnValueChanged(Value);
            }
        }
    }
}
