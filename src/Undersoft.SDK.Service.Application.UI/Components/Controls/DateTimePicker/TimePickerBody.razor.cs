using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class TimePickerBody
{
    private TimeSpan CurrentTime { get; set; }

    private string? ClassString => CssBuilder.Default("time-panel")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public TimeSpan Value { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ValueChanged { get; set; }

    [Parameter]
    [NotNull]
    public string? CancelButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    public Action? OnClose { get; set; }

    [Parameter]
    public Action? OnConfirm { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<DateTimePicker<DateTime>>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        CurrentTime = Value;
        CancelButtonText ??= Localizer[nameof(CancelButtonText)];
        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];
    }

    private Task OnClickClose()
    {
        CurrentTime = Value;
        OnClose?.Invoke();
        return Task.CompletedTask;
    }

    private async Task OnClickConfirm()
    {
        Value = CurrentTime;
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        OnConfirm?.Invoke();
    }
}
