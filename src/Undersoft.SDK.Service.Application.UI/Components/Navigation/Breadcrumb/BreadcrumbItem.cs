namespace Undersoft.SDK.Service.Application.Components;

public class BreadcrumbItem
{
    public string? Url { get; }

    public string Text { get; }

    public BreadcrumbItem(string text, string? url = null)
    {
        Text = text;
        Url = url;
    }
}
