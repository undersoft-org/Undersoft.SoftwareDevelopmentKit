namespace Undersoft.SDK.Service.Application.Components;

public class Popover : Tooltip
{
    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public bool ShowShadow { get; set; } = true;

    protected override string? CustomClassString => CssBuilder.Default(CustomClass)
        .AddClass("shadow", ShowShadow)
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToggleString = "popover";
    }

    protected override async Task ModuleInitAsync()
    {
        if (!string.IsNullOrEmpty(Content))
        {
            await InvokeInitAsync(Id, Title, Content);
        }
    }
}
