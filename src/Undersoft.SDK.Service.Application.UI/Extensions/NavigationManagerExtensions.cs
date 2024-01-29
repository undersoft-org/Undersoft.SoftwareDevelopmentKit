using Microsoft.Extensions.DependencyInjection;

namespace Undersoft.SDK.Service.Application.Components;

public static class NavigationManagerExtensions
{
    public static void NavigateTo(this NavigationManager navigation, IServiceProvider provider, string url, string text, string? icon = null, bool closable = true)
    {
        var option = provider.GetRequiredService<TabItemTextOptions>();
        option.Text = text;
        option.Icon = icon;
        option.Closable = closable;
        navigation.NavigateTo(url);
    }
}
