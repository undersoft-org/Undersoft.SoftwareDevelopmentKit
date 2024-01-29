namespace Undersoft.SDK.Service.Application.Components;

internal class DefaultIconTheme : IIconTheme
{
    private IOptions<IconThemeOptions> Options { get; set; }

    public DefaultIconTheme(IOptions<IconThemeOptions> options)
    {
        Options = options;
    }

    public Dictionary<ComponentIcons, string> GetIcons()
    {
        if (!Options.Value.Icons.TryGetValue(Options.Value.ThemeKey, out var icons))
        {
            icons = new Dictionary<ComponentIcons, string>();
        }
        return icons;
    }
}
