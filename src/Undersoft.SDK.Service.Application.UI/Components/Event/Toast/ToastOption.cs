namespace Undersoft.SDK.Service.Application.Components;

public class ToastOption : PopupOptionBase
{
    internal Toast? Toast { get; set; }

    public ToastCategory Category { get; set; }

    public string? SuccessIcon { get; set; }

    public string? InformationIcon { get; set; }

    public string? ErrorIcon { get; set; }

    public string? WarningIcon { get; set; }

    public string? Title { get; set; }

    public RenderFragment? ChildContent { get; set; }

    public bool ShowClose { get; set; } = true;

    public bool ShowHeader { get; set; } = true;

    public RenderFragment? HeaderTemplate { get; set; }

    public bool Animation { get; set; } = true;

    public void Close()
    {
        Toast?.Close();
    }
}
