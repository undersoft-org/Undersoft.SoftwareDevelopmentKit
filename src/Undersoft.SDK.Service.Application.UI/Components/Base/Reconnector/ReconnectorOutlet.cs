using Microsoft.AspNetCore.Components.Rendering;

namespace Microsoft.AspNetCore.Components.Web;

public class ReconnectorOutlet : ComponentBase
{
    [Parameter]
    public bool AutoReconnect { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<ReconnectorContent>(0);
        builder.AddAttribute(1, nameof(ReconnectorContent.AutoReconnect), AutoReconnect);
        builder.CloseComponent();
    }
}
