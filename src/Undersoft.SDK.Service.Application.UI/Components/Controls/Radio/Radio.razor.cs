namespace Undersoft.SDK.Service.Application.Components;

public partial class Radio<TValue>
{
    [Parameter]
    public Func<TValue, Task>? OnClick { get; set; }

    [Parameter]
    public bool IsButton { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? GroupName { get; set; }

    private async Task OnClickHandler()
    {
        if (OnClick != null)
        {
            await OnClick(Value);
        }
    }
}
