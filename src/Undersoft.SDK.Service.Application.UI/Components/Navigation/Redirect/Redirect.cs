namespace Undersoft.SDK.Service.Application.Components;

public class Redirect : ComponentBase
{
    [Inject]
    [NotNull]
    private NavigationManager? Navigation { get; set; }

    [Parameter]
    public string Url { get; set; } = "Account/Login";

    protected override void OnAfterRender(bool firstRender)
    {
        Navigation.NavigateTo(Url, true);
    }
}
