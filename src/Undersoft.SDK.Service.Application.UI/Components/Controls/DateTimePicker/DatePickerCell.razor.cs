namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class DatePickerCell
{
    private string? ClassString => CssBuilder.Default("cell")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public DateTime Value { get; set; }

    [Parameter]
    [NotNull]
    public string? Text { get; set; }

    [Parameter]
    [NotNull]
    public Func<DateTime, Task>? OnClick { get; set; }
}
