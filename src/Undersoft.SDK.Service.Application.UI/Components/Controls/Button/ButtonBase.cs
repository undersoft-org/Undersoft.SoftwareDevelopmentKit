using Microsoft.AspNetCore.Components.Web;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("button", ModuleName = "Button")]
public abstract class ButtonBase : TooltipWrapperBase
{
    protected string? ClassName => CssBuilder.Default("btn")
        .AddClass($"btn-outline-{Color.ToDescriptionString()}", IsOutline)
        .AddClass($"btn-{Color.ToDescriptionString()}", !IsOutline && Color != Color.None)
        .AddClass($"btn-{Size.ToDescriptionString()}", Size != Size.None)
        .AddClass("btn-block", IsBlock)
        .AddClass("btn-round", ButtonStyle == ButtonStyle.Round)
        .AddClass("btn-circle", ButtonStyle == ButtonStyle.Circle)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? Disabled => IsDisabled ? "disabled" : null;

    protected string DisabledString => IsDisabled ? "true" : "false";

    protected string? Tab => IsDisabled ? "-1" : null;

    protected string? ButtonIcon { get; set; }

    [Parameter]
    public ButtonStyle ButtonStyle { get; set; }

    [Parameter]
    public ButtonType ButtonType { get; set; } = ButtonType.Button;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public Func<Task>? OnClickWithoutRender { get; set; }

    [Parameter]
    public virtual Color Color { get; set; } = Color.Primary;

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    [NotNull]
    public string? LoadingIcon { get; set; }

    [Parameter]
    public bool IsAsync { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public bool IsOutline { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.None;

    [Parameter]
    public bool IsBlock { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public bool StopPropagation { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    protected ValidateForm? ValidateForm { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected bool IsAsyncLoading { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ButtonIcon = Icon;

        if (IsAsync && ValidateForm != null)
        {
            ValidateForm.RegisterAsyncSubmitButton(this);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        LoadingIcon ??= IconTheme.GetIconByKey(ComponentIcons.ButtonLoadingIcon);

        if (!IsAsyncLoading)
        {
            ButtonIcon = Icon;
        }

        if (Tooltip != null && !string.IsNullOrEmpty(TooltipText))
        {
            Tooltip.SetParameters(TooltipText, TooltipPlacement, TooltipTrigger);
        }
    }

    private bool _prevDisable;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _prevDisable = IsDisabled;
            if (!IsDisabled)
            {
                await ShowTooltip();
            }
        }
        else if (_prevDisable != IsDisabled)
        {
            _prevDisable = IsDisabled;
            if (IsDisabled)
            {
                await RemoveTooltip();
            }
            else
            {
                await ShowTooltip();
            }
        }
    }

    public void SetDisable(bool disable)
    {
        IsDisabled = disable;
        StateHasChanged();
    }

    internal void TriggerAsync(bool loading)
    {
        IsAsyncLoading = loading;
        ButtonIcon = loading ? LoadingIcon : Icon;
        SetDisable(loading);
    }

    public virtual async Task ShowTooltip()
    {
        if (Tooltip == null && !string.IsNullOrEmpty(TooltipText))
        {
            await InvokeExecuteAsync(Id, "showTooltip", TooltipText);
        }
    }

    public virtual async Task RemoveTooltip()
    {
        if (Tooltip == null)
        {
            await InvokeExecuteAsync(Id, "removeTooltip");
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await RemoveTooltip();
        }
        await base.DisposeAsync(disposing);
    }
}
