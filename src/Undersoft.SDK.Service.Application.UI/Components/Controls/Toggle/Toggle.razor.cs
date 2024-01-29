using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Toggle
{
    private string? ClassName => CssBuilder.Default("btn btn-toggle")
        .AddClass("off", !Value)
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? ToggleOnClassString => CssBuilder.Default("toggle on")
        .AddClass($"bg-{Color.ToDescriptionString()}", Color != Color.None)
        .Build();

    private string? WrapperClassString => CssBuilder.Default("toggle")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public Color Color { get; set; } = Color.Success;

    [Inject]
    [NotNull]
    private IStringLocalizer<Toggle>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnText ??= Localizer[nameof(OnText)];
        OffText ??= Localizer[nameof(OffText)];
    }

    private async Task OnClick()
    {
        if (!IsDisabled)
        {
            Value = !Value;
            if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(Value);
            OnValueChanged?.Invoke(Value);
        }
    }
}
