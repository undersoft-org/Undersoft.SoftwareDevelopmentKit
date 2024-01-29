namespace Undersoft.SDK.Service.Application.Components;

public partial class Drawer
{
    private string? ClassString => CssBuilder.Default("drawer collapse")
        .AddClass("no-bd", !ShowBackdrop)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? DrawerStyleString => CssBuilder.Default()
        .AddClass($"width: {Width};", !string.IsNullOrEmpty(Width) && Placement != Placement.Top && Placement != Placement.Bottom)
        .AddClass($"height: {Height};", !string.IsNullOrEmpty(Height) && (Placement == Placement.Top || Placement == Placement.Bottom))
        .Build();

    private string? DrawerClassString => CssBuilder.Default("drawer-body")
        .AddClass("left", Placement != Placement.Right && Placement != Placement.Top && Placement != Placement.Bottom)
        .AddClass("top", Placement == Placement.Top)
        .AddClass("right", Placement == Placement.Right)
        .AddClass("bottom", Placement == Placement.Bottom)
        .Build();

    [Parameter]
    public string Width { get; set; } = "360px";

    [Parameter]
    public string Height { get; set; } = "290px";

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public Func<Task>? OnClickBackdrop { get; set; }

    [Parameter]
    public bool IsBackdrop { get; set; }

    [Parameter]
    public bool ShowBackdrop { get; set; } = true;

    [Parameter]
    public Placement Placement { get; set; } = Placement.Left;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            await InvokeVoidAsync("execute", Id, IsOpen);
        }
    }

    public async Task OnContainerClick()
    {
        if (IsBackdrop)
        {
            await Close();
            if (OnClickBackdrop != null) await OnClickBackdrop.Invoke();
        }
    }

    public async Task Close()
    {
        IsOpen = false;
        if (IsOpenChanged.HasDelegate)
        {
            await IsOpenChanged.InvokeAsync(IsOpen);
        }
        else
        {
            StateHasChanged();
        }
    }
}
