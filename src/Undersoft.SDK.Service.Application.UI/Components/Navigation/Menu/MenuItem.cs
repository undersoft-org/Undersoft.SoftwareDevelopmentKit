using Microsoft.AspNetCore.Components.Routing;

namespace Undersoft.SDK.Service.Application.Components;

public class MenuItem : NodeItem
{
    public MenuItem? Parent { get; set; }

    public IEnumerable<MenuItem> Items { get; set; } = Enumerable.Empty<MenuItem>();

    public string? Url { get; set; }

    public string? Target { get; set; }

    public NavLinkMatch Match { get; set; } = NavLinkMatch.All;

    public int Indent { get; private set; }

    public MenuItem() { }

    public MenuItem(string text, string? url = null, string? icon = null)
    {
        Text = text;
        Url = url;
        Icon = icon;
    }

    protected internal virtual void SetIndent()
    {
        if (Parent != null)
        {
            Indent = Parent.Indent + 1;
        }
    }

    public IEnumerable<MenuItem> GetAllSubItems() => Items.Concat(GetSubItems(Items));

    private static IEnumerable<MenuItem> GetSubItems(IEnumerable<MenuItem> items) => items.SelectMany(i => i.Items.Any() ? i.Items.Concat(GetSubItems(i.Items)) : i.Items);
}
