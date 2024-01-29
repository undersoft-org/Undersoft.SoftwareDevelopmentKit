namespace Undersoft.SDK.Service.Application.Components;

public partial class Alert
{
    protected override string? ClassName => CssBuilder.Default(base.ClassName)
        .AddClass("d-none", !IsShown)
        .AddClass("shadow", ShowShadow)
        .AddClass($"border-{Color.ToDescriptionString()}", ShowBorder)
        .Build();

    private bool IsShown { get; set; } = true;

    [Parameter]
    public bool ShowShadow { get; set; }

    [Parameter]
    public bool ShowBorder { get; set; }

    private async Task OnClick()
    {
        IsShown = !IsShown;
        if (OnDismiss != null) await OnDismiss();
    }
}
