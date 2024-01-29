namespace Undersoft.SDK.Service.Application.Components;

public partial class PopConfirmButtonContent
{
    private string? CloseButtonClass => CssBuilder.Default("btn btn-xs")
        .AddClass($"btn-{CloseButtonColor.ToDescriptionString()}")
        .Build();

    private string? ConfirmButtonClass => CssBuilder.Default("btn btn-xs")
        .AddClass($"btn-{ConfirmButtonColor.ToDescriptionString()}")
        .Build();

    private string? IconString => CssBuilder.Default("text-info")
        .AddClass(Icon)
        .Build();

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? CloseButtonText { get; set; }

    [Parameter]
    public Color CloseButtonColor { get; set; } = Color.Secondary;

    [Parameter]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    public Color ConfirmButtonColor { get; set; } = Color.Primary;

    [Parameter]
    [NotNull]
    public string? Icon { get; set; }

    [Parameter]
    public Func<Task>? OnConfirm { get; set; }

    [Parameter]
    public Func<Task>? OnClose { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.PopConfirmButtonConfirmIcon);
    }

    public async Task OnCloseClick()
    {
        if (OnClose != null)
        {
            await OnClose();
        }
    }

    public async Task OnConfirmClick()
    {
        if (OnConfirm != null)
        {
            await OnConfirm();
        }
    }
}
