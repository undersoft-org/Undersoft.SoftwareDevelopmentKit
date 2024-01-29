namespace Undersoft.SDK.Service.Application.Components;

public class SwalOption : PopupOptionBase
{
    internal Modal? Modal { get; set; }

    internal TaskCompletionSource<bool> ReturnTask { get; } = new TaskCompletionSource<bool>();

    internal bool IsConfirm { get; set; }

    public SwalCategory Category { get; set; }

    public string? Title { get; set; }

    public RenderFragment? BodyTemplate { get; set; }

    public RenderFragment? FooterTemplate { get; set; }

    public bool ShowClose { get; set; } = true;

    public bool ShowFooter { get; set; }

    public RenderFragment? ButtonTemplate { get; set; }

    public string? CloseButtonIcon { get; set; }

    public string? ConfirmButtonIcon { get; set; }

    public string? CloseButtonText { get; set; }

    public string? ConfirmButtonText { get; set; }

    public string? CancelButtonText { get; set; }

    public string? Class { get; set; }

    public Func<Task>? OnCloseAsync { get; set; }

    public Func<Task>? OnConfirmAsync { get; set; }

    public SwalOption()
    {
        IsAutoHide = false;
    }

    public Dictionary<string, object> ToAttributes()
    {
        var parameters = new Dictionary<string, object>
        {
            [nameof(Size)] = Size.Medium,
            [nameof(ModalDialog.IsCentered)] = true,
            [nameof(ModalDialog.IsScrolling)] = false,
            [nameof(ModalDialog.ShowCloseButton)] = false,
            [nameof(ModalDialog.ShowHeader)] = false,
            [nameof(ModalDialog.ShowFooter)] = false
        };

        if (!string.IsNullOrEmpty(Title))
        {
            parameters.Add(nameof(ModalDialog.Title), Title);
        }

        if (!string.IsNullOrEmpty(Class))
        {
            parameters.Add(nameof(ModalDialog.Class), Class);
        }
        return parameters;
    }

    public async Task CloseAsync(bool returnValue = true)
    {
        if (Modal != null)
        {
            await Modal.Close();
        }

        if (IsConfirm)
        {
            ReturnTask.TrySetResult(returnValue);
        }
    }
}
