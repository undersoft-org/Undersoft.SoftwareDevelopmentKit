namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class Step
{
    private string? ClassString => CssBuilder.Default("step is-horizontal")
        .AddClass("is-flex", IsLast && !((Steps?.IsCenter ?? false) || IsCenter))
        .AddClass("is-center", (Steps?.IsCenter ?? false) || IsCenter)
        .Build();

    private string? StyleString => CssBuilder.Default("margin-right: 0px;")
        .AddClass($"flex-basis: {Space};", !string.IsNullOrEmpty(Space))
        .Build();

    private string? HeadClassString => CssBuilder.Default("step-head")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? LineStyleString => CssBuilder.Default()
        .AddClass("transition-delay: 150ms; border-width: 1px; width: 100%;", Status == StepStatus.Finish || Status == StepStatus.Success)
        .Build();

    private string? StepIconClassString => CssBuilder.Default("step-icon")
        .AddClass("is-text", !IsIcon)
        .AddClass("is-icon", IsIcon)
        .Build();

    private string? IconClassString => CssBuilder.Default("step-icon-inner")
        .AddClass(Icon, IsIcon || Status == StepStatus.Finish || Status == StepStatus.Success)
        .AddClass(ErrorStepIcon, IsIcon || Status == StepStatus.Error)
        .AddClass("is-status", !IsIcon && (Status == StepStatus.Finish || Status == StepStatus.Success || Status == StepStatus.Error))
        .Build();

    private string? TitleClassString => CssBuilder.Default("step-title")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? DescClassString => CssBuilder.Default("step-description")
        .AddClass($"is-{Status.ToDescriptionString()}")
        .Build();

    private string? StepString => (Status == StepStatus.Process || Status == StepStatus.Wait) && !IsIcon ? (StepIndex + 1).ToString() : null;

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    public string? ErrorIcon { get; set; }

    [Parameter]
    public string? ErrorStepIcon { get; set; }

    [Parameter]
    public StepStatus Status { get; set; }

    [Parameter]
    public string? Description { get; set; }

    [Parameter]
    public string? Space { get; set; }

    [Parameter]
    public bool IsIcon { get; set; }

    [Parameter]
    public bool IsLast { get; set; }

    [Parameter]
    public bool IsCenter { get; set; }

    [Parameter]
    public int StepIndex { get; set; }

    [CascadingParameter]
    private Steps? Steps { get; set; }

    [Parameter]
    public Action<StepStatus>? OnStatusChanged { get; set; }

    [Parameter]
    public RenderFragment? DescriptionTemplate { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.StepIcon);
        ErrorIcon = IconTheme.GetIconByKey(ComponentIcons.StepErrorIcon);
    }
}
