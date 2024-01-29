using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class Split
{
    private ElementReference SplitElement { get; set; }

    private string? ClassString => CssBuilder.Default("split")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? WrapperClassString => CssBuilder.Default("split-wrapper")
        .AddClass("is-horizontal", !IsVertical)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"flex-basis: {Basis.ConvertToPercentString()};")
        .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync(SplitElement, "bb_split");
        }
    }
}
