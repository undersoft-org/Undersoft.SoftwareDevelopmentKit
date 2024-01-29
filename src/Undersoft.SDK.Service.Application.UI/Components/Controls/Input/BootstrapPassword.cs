namespace Undersoft.SDK.Service.Application.Components;

public class BootstrapPassword : BootstrapInput<string>
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Type = "password";
    }
}
