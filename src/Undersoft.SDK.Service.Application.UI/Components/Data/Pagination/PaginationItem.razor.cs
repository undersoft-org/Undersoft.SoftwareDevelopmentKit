namespace Undersoft.SDK.Service.Application.Components;

public partial class PaginationItem
{
    [Parameter]
    public EventCallback<int> OnClick { get; set; }

    [Parameter]
    public int Index { get; set; }

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string? ClassString => CssBuilder.Default("page-item")
        .AddClass("active", IsActive)
        .AddClass("disabled", IsDisabled)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private async Task OnClickItem()
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(Index);
        }
    }
}
