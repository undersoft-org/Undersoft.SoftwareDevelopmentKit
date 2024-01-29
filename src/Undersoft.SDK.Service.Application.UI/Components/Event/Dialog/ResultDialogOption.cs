namespace Undersoft.SDK.Service.Application.Components;

public class ResultDialogOption : DialogOption
{
    public ResultDialogOption()
    {
        ShowCloseButton = false;
    }

    public bool ShowYesButton { get; set; } = true;

    public string? ButtonYesText { get; set; }

    public string? ButtonYesIcon { get; set; }

    public Color ButtonYesColor { get; set; } = Color.Primary;

    public bool ShowNoButton { get; set; } = true;

    public string? ButtonNoText { get; set; }

    public string? ButtonNoIcon { get; set; }

    public Color ButtonNoColor { get; set; } = Color.Danger;

    public string? ButtonCloseText { get; set; }

    public string? ButtonCloseIcon { get; set; }

    public Color ButtonCloseColor { get; set; } = Color.Secondary;

    public Dictionary<string, object>? ComponentParamters { get; set; }

    internal TaskCompletionSource<DialogResult> ReturnTask { get; } = new TaskCompletionSource<DialogResult>();
}
