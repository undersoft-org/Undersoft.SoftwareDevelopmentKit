namespace Undersoft.SDK.Service.Application.Components;

public partial class Tag
{
    protected override string? ClassName => CssBuilder.Default("tag fade show")
        .AddClass($"alert-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private async Task OnClick()
    {
        if (OnDismiss != null) await OnDismiss();
    }
}
