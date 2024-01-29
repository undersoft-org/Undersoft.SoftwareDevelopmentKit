namespace Undersoft.SDK.Service.Application.Components;

public partial class Toast
{
    private string? AutoHide => Options.IsAutoHide ? null : "false";

    private string? ClassString => CssBuilder.Default("toast")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ProgressClass => CssBuilder.Default("toast-progress")
        .AddClass($"bg-{Options.Category.ToDescriptionString()}")
        .Build();

    private string? IconString => CssBuilder.Default()
        .AddClass(Options.SuccessIcon, Options.Category == ToastCategory.Success)
        .AddClass(Options.InformationIcon, Options.Category == ToastCategory.Information)
        .AddClass(Options.ErrorIcon, Options.Category == ToastCategory.Error)
        .AddClass(Options.WarningIcon, Options.Category == ToastCategory.Warning)
        .Build();

    private string? IconBarString => CssBuilder.Default("toast-bar me-2")
        .AddClass("text-success", Options.Category == ToastCategory.Success)
        .AddClass("text-info", Options.Category == ToastCategory.Information)
        .AddClass("text-danger", Options.Category == ToastCategory.Error)
        .AddClass("text-warning", Options.Category == ToastCategory.Warning)
        .Build();

    protected string? DelayString => Options.IsAutoHide ? Convert.ToString(Options.Delay) : null;

    protected string? AnimationString => Options.Animation ? null : "false";

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public ToastOption? Options { get; set; }

    [CascadingParameter]
    protected ToastContainer? ToastContainer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Options.Toast = this;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Options.SuccessIcon ??= IconTheme.GetIconByKey(ComponentIcons.ToastSuccessIcon);
        Options.InformationIcon ??= IconTheme.GetIconByKey(ComponentIcons.ToastInformationIcon);
        Options.WarningIcon ??= IconTheme.GetIconByKey(ComponentIcons.ToastWarningIcon);
        Options.ErrorIcon ??= IconTheme.GetIconByKey(ComponentIcons.ToastErrorIcon);
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, nameof(Close));

    [JSInvokable]
    public void Close()
    {
        ToastContainer?.Close(Options);
    }
}
