using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Empty
{
    private string? ClassString => CssBuilder.Default("empty")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public string? Image { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public RenderFragment? Template { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Empty>? Localizer { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Text ??= Localizer[nameof(Text)];
    }
}
