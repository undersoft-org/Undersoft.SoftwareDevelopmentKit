namespace Undersoft.SDK.Service.Application.Components;

public static class IIconThemeExtensions
{
    public static string? GetIconByKey(this IIconTheme iconTheme, ComponentIcons key, string? defaultIcon = null)
    {
        string? icon = null;
        var icons = iconTheme.GetIcons();
        if (icons.TryGetValue(key, out var v))
        {
            icon = v;
        }
        return icon ?? defaultIcon;
    }
}
