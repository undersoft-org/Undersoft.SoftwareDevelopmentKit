namespace Undersoft.SDK.Service.Application.Components;

public class MessageOption : PopupOptionBase
{
    public Color Color { get; set; } = Color.Primary;

    public bool ShowDismiss { get; set; }

    public string? Icon { get; set; }

    public bool ShowBar { get; set; }

    public Func<Task>? OnDismiss { get; set; }
}
