namespace Undersoft.SDK.Service.Application.Components;

public abstract class PopConfirmButtonBase : ButtonBase
{
    protected override string? PlacementString => Placement != Placement.Auto ? Placement.ToDescriptionString() : null;

    protected override string? TriggerString => Trigger == "click" ? null : Trigger;

    [Parameter]
    public bool IsLink { get; set; }

    [Parameter]
    public Placement Placement { get; set; }

    [Parameter]
    public string? Trigger { get; set; }

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    [NotNull]
    public RenderFragment? BodyTemplate { get; set; }

    [Parameter]
    [NotNull]
    public Func<Task>? OnConfirm { get; set; }

    [Parameter]
    [NotNull]
    public Func<Task>? OnClose { get; set; }

    [Parameter]
    [NotNull]
    public Func<Task<bool>>? OnBeforeClick { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public Color CloseButtonColor { get; set; } = Color.Secondary;

    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    public Color ConfirmButtonColor { get; set; } = Color.Primary;

    [Parameter]
    [NotNull]
    public string? ConfirmIcon { get; set; }

    [Parameter]
    public string? CustomClass { get; set; }

    [Parameter]
    public bool ShowShadow { get; set; } = true;

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ConfirmIcon ??= IconTheme.GetIconByKey(ComponentIcons.PopConfirmButtonConfirmIcon);
        Trigger ??= "click";

        OnClose ??= () => Task.CompletedTask;
        OnConfirm ??= () => Task.CompletedTask;
        OnBeforeClick ??= () => Task.FromResult(true);
    }
}
