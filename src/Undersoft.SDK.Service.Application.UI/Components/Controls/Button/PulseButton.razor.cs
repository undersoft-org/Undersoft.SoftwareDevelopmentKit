namespace Undersoft.SDK.Service.Application.Components;

public partial class PulseButton
{
    [Parameter]
    public string? ImageUrl { get; set; }

    [Parameter]
    public Color PulseColor { get; set; } = Color.Warning;

    private string? ButtonClassName => CssBuilder.Default(ClassName)
        .AddClass("btn-pulse")
        .Build();

    private string? PulseColorString => CssBuilder.Default("pulse-ring border")
        .AddClass($"border-{PulseColor.ToDescriptionString()}", PulseColor != Color.None)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ButtonStyle == ButtonStyle.None)
        {
            ButtonStyle = ButtonStyle.Circle;
        }
    }
}
