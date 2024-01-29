namespace Undersoft.SDK.Service.Application.Components;

public class DialogOption
{
    [Obsolete("CloseDialogAsync")]
    [ExcludeFromCodeCoverage]
    public Modal? Dialog { get; set; }

    internal Modal? Modal { get; set; }

    public string? Title { get; set; }

    public string? Class { get; set; }

    public Size Size { get; set; } = Size.ExtraExtraLarge;

    public FullScreenSize FullScreenSize { get; set; } = FullScreenSize.None;

    public bool ShowMaximizeButton { get; set; }

    public bool IsCentered { get; set; } = true;

    public bool IsScrolling { get; set; } = false;

    public bool ShowCloseButton { get; set; } = true;

    public bool ShowHeaderCloseButton { get; set; } = true;

    public bool IsKeyboard { get; set; } = true;

    public bool IsBackdrop { get; set; }

    public bool ShowFooter { get; set; } = true;

    public bool ShowPrintButton { get; set; }

    public bool ShowSaveButton { get; set; }

    public bool ShowPrintButtonInHeader { get; set; }

    public string? PrintButtonText { get; set; }

    public object? BodyContext { get; set; }

    public RenderFragment? BodyTemplate { get; set; }

    public RenderFragment? FooterTemplate { get; set; }

    public RenderFragment? HeaderTemplate { get; set; }

    public RenderFragment? HeaderToolbarTemplate { get; set; }

    public BootstrapDynamicComponent? Component { get; set; }

    public Func<Task>? OnCloseAsync { get; set; }

    public Func<Task<bool>>? OnSaveAsync { get; set; }

    public string? CloseButtonText { get; set; }

    public string? SaveButtonText { get; set; }

    public bool IsAutoCloseAfterSave { get; set; } = true;

    public bool IsDraggable { get; set; }

    public Func<Task>? OnShownAsync { get; set; }

    public async Task CloseDialogAsync()
    {
        if (Modal != null)
        {
            await Modal.Close();
        }
    }

    public Dictionary<string, object> ToAttributes()
    {
        var ret = new Dictionary<string, object>
        {
            [nameof(ModalDialog.Size)] = Size,
            [nameof(ModalDialog.FullScreenSize)] = FullScreenSize,
            [nameof(ModalDialog.IsCentered)] = IsCentered,
            [nameof(ModalDialog.IsScrolling)] = IsScrolling,
            [nameof(ModalDialog.ShowCloseButton)] = ShowCloseButton,
            [nameof(ModalDialog.ShowSaveButton)] = ShowSaveButton,
            [nameof(ModalDialog.ShowHeaderCloseButton)] = ShowHeaderCloseButton,
            [nameof(ModalDialog.ShowFooter)] = ShowFooter,
            [nameof(ModalDialog.ShowPrintButton)] = ShowPrintButton,
            [nameof(ModalDialog.ShowPrintButtonInHeader)] = ShowPrintButtonInHeader,
            [nameof(ModalDialog.IsAutoCloseAfterSave)] = IsAutoCloseAfterSave,
            [nameof(ModalDialog.IsDraggable)] = IsDraggable,
            [nameof(ModalDialog.ShowMaximizeButton)] = ShowMaximizeButton
        };
        if (!string.IsNullOrEmpty(PrintButtonText))
        {
            ret.Add(nameof(ModalDialog.PrintButtonText), PrintButtonText);
        }
        if (!string.IsNullOrEmpty(Title))
        {
            ret.Add(nameof(ModalDialog.Title), Title);
        }
        if (BodyContext != null)
        {
            ret.Add(nameof(ModalDialog.BodyContext), BodyContext);
        }
        return ret;
    }
}
