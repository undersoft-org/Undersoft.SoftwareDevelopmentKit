namespace Undersoft.SDK.Service.Application.Components;

public partial class FileIcon
{
    private string? ClassString => CssBuilder.Default("file-icon")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? IconClassString => CssBuilder.Default("badge")
        .AddClass($"bg-{IconColor.ToDescriptionString()}", IconColor != Color.None)
        .Build();

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? Extension { get; set; }

    [Parameter]
    public RenderFragment? BackgroundTemplate { get; set; }

    [Parameter]
    public Color IconColor { get; set; } = Color.Primary;
}
