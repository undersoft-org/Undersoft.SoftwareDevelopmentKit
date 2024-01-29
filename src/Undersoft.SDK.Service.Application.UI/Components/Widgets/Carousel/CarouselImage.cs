using Microsoft.AspNetCore.Components.Rendering;

namespace Undersoft.SDK.Service.Application.Components;

internal class CarouselImage : ComponentBase
{
    [Parameter]
    public string? ImageUrl { get; set; }

    [Parameter]
    public Func<string, Task>? OnClick { get; set; }

    private async Task OnClickImage()
    {
        if (OnClick != null)
        {
            await OnClick(ImageUrl ?? "");
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "img");
        if (!string.IsNullOrEmpty(ImageUrl))
        {
            builder.AddAttribute(1, "src", ImageUrl);
        }
        if (OnClick != null)
        {
            builder.AddAttribute(2, "onclick", OnClickImage);
        }
        builder.CloseElement();
    }
}
