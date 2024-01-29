namespace Undersoft.SDK.Service.Application.Components;

public static class MenuItemExtensions
{
    public static void CascadingSetActive(this MenuItem item, bool active = true)
    {
        item.IsActive = active;
        var current = item;
        while (current.Parent != null)
        {
            current.Parent.IsActive = active;
            current.Parent.IsCollapsed = false;
            current = current.Parent;
        }
    }
}
