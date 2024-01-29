namespace Undersoft.SDK.Service.Application.Components;

public partial class FullScreenButton
{
    [Parameter]
    public string? FullScreenIcon { get; set; }

    [Inject]
    [NotNull]
    private FullScreenService? FullScreenService { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? ClassString => CssBuilder.Default("btn btn-fs")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ButtonIconString => CssBuilder.Default()
        .AddClass(Icon)
        .AddClass("fs-off", !string.IsNullOrEmpty(FullScreenIcon))
        .Build();

    private string? FullScreenIconString => CssBuilder.Default("fs-on")
        .AddClass(FullScreenIcon)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.FullScreenButtonIcon);
    }

    private Task ToggleFullScreen() => FullScreenService.Toggle();
}
