namespace Undersoft.SDK.Service.Application.Components;

internal class MarkdownOption
{
    public string InitialEditType { get; set; } = "markdown";

    public string PreviewStyle { get; set; } = "vertical";

    public string Height { get; set; } = "300px";

    public string MinHeight { get; set; } = "200px";

    public string? Language { get; set; }

    public string? Placeholder { get; set; }

    public string? InitialValue { get; set; }

    public bool? Viewer { get; set; } = false;

    public string? Theme { get; set; }

    public bool? EnableHighlight { get; set; }

    public bool Autofocus { get; set; } = false;
}
