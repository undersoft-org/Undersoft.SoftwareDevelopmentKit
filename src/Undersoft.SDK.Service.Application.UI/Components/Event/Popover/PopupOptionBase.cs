namespace Undersoft.SDK.Service.Application.Components;

public abstract class PopupOptionBase
{
    public string? Content { get; set; }

    public bool IsAutoHide { get; set; } = true;

    public int Delay { get; set; } = 4000;

    public bool ForceDelay { get; set; }
}
