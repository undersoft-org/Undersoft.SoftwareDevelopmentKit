using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class ModalDialog : IHandlerException, IDisposable
{
    private string MaximizeAriaLabel => MaximizeStatus ? "maximize" : "restore";

    private string? ClassName => CssBuilder.Default("modal-dialog")
        .AddClass("modal-dialog-centered", IsCentered && !IsDraggable)
        .AddClass($"modal-{Size.ToDescriptionString()}", Size != Size.None && FullScreenSize != FullScreenSize.Always && !MaximizeStatus)
        .AddClass($"modal-{FullScreenSize.ToDescriptionString()}", FullScreenSize != FullScreenSize.None && !MaximizeStatus)
        .AddClass("modal-dialog-scrollable", IsScrolling)
        .AddClass("modal-fullscreen", MaximizeStatus)
        .AddClass("is-draggable", IsDraggable)
        .AddClass("d-none", !IsShown)
        .AddClass(Class, !string.IsNullOrEmpty(Class))
        .Build();

    internal bool IsShown { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.ExtraExtraLarge;

    [Parameter]
    public FullScreenSize FullScreenSize { get; set; }

    [Parameter]
    public bool IsCentered { get; set; }

    [Parameter]
    public bool IsScrolling { get; set; }

    [Parameter]
    public bool IsDraggable { get; set; }

    [Parameter]
    public bool ShowMaximizeButton { get; set; }

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter]
    public bool ShowSaveButton { get; set; }

    [Parameter]
    public bool ShowPrintButton { get; set; }

    [Parameter]
    public bool ShowHeaderCloseButton { get; set; } = true;

    [Parameter]
    public bool ShowHeader { get; set; } = true;

    [Parameter]
    public bool ShowFooter { get; set; } = true;

    [Parameter]
    public bool ShowPrintButtonInHeader { get; set; }

    [Parameter]
    public string? PrintButtonText { get; set; }

    [Parameter]
    public object? BodyContext { get; set; }

    [Parameter]
    public RenderFragment? HeaderToolbarTemplate { get; set; }

    [Parameter]
    public RenderFragment? BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public Func<Task<bool>>? OnSaveAsync { get; set; }

    [Parameter]
    public bool IsAutoCloseAfterSave { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? MaximizeWindowIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? RestoreWindowIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveIcon { get; set; }

    [CascadingParameter]
    [NotNull]
    protected Modal? Modal { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<ModalDialog>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? MaximizeIconString { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ErrorLogger?.Register(this);
        Modal.AddDialog(this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        SaveButtonText ??= Localizer[nameof(SaveButtonText)];
        PrintButtonText ??= Localizer[nameof(PrintButtonText)];

        CloseButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogCloseButtonIcon);
        MaximizeWindowIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogMaxminzeWindowIcon);
        SaveIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogSaveButtonIcon);
        RestoreWindowIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogRestoreWindowIcon);

        MaximizeIconString = MaximizeWindowIcon;
    }

    public void SetHeaderText(string text)
    {
        Title = text;
        StateHasChanged();
    }

    private async Task OnClickClose() => await Modal.Close();

    private bool MaximizeStatus { get; set; }

    private void OnToggleMaximize()
    {
        MaximizeStatus = !MaximizeStatus;
        MaximizeIconString = MaximizeStatus ? RestoreWindowIcon : MaximizeWindowIcon;
    }

    private async Task OnClickSave()
    {
        var ret = true;
        if (OnSaveAsync != null)
        {
            await OnSaveAsync();
        }
        if (IsAutoCloseAfterSave && ret)
        {
            await OnClickClose();
        }
    }

    private RenderFragment RenderBodyTemplate() => builder =>
    {
        builder.AddContent(0, _errorContent ?? BodyTemplate);
        _errorContent = null;
    };

    protected RenderFragment? _errorContent;

    public virtual Task HandlerException(Exception ex, RenderFragment<Exception> errorContent)
    {
        _errorContent = errorContent(ex);
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ErrorLogger?.UnRegister(this);
            Modal.RemoveDialog(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
