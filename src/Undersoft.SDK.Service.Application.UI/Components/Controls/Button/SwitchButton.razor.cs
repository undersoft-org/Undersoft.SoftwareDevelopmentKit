using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class SwitchButton
{
    [Parameter]
    [NotNull]
    public string? OnText { get; set; }

    [Parameter]
    [NotNull]
    public string? OffText { get; set; }

    [Parameter]
    public bool ToggleState { get; set; }

    [Parameter]
    public EventCallback<bool> ToggleStateChanged { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<SwitchButton>? Localizer { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        OnText ??= Localizer[nameof(OnText)];
        OffText ??= Localizer[nameof(OffText)];
    }

    private async Task OnToggle()
    {
        ToggleState = !ToggleState;
        if (ToggleStateChanged.HasDelegate)
        {
            await ToggleStateChanged.InvokeAsync(ToggleState);
        }
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }

    private string? GetText() => ToggleState ? OnText : OffText;
}
