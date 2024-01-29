using Microsoft.Extensions.Localization;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class GoTop
{
    private ElementReference GoTopElement { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public string? Target { get; set; }

    [Parameter]
    [NotNull]
    public string? TooltipText { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<GoTop>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        TooltipText ??= Localizer[nameof(TooltipText)];
        Icon ??= IconTheme.GetIconByKey(ComponentIcons.GoTopIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender) await JSRuntime.InvokeVoidAsync(GoTopElement, "bb_gotop", Target ?? "");
    }
}
