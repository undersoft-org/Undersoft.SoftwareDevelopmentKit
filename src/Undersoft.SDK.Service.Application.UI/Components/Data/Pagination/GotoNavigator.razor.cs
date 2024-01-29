using Microsoft.AspNetCore.Components.Web;

namespace Undersoft.SDK.Service.Application.Components;

public partial class GotoNavigator
{
    [Parameter]
    public int Index { get; set; }

    [Parameter]
    public string? GotoText { get; set; }

    [Parameter]
    public Func<int, Task>? OnNavigation { get; set; }

    private async Task OnValueChanged(int val)
    {
        Index = val;
        if (OnNavigation != null)
        {
            await OnNavigation(Index);
        }
    }

    private async Task OnKeyup(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await OnValueChanged(Index);
        }
    }
}
