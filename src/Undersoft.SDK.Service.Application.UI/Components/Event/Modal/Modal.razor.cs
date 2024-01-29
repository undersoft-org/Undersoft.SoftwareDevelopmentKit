namespace Undersoft.SDK.Service.Application.Components;

public partial class Modal
{
    private string? ClassString => CssBuilder.Default("modal")
        .AddClass("fade", IsFade)
        .Build();

    protected List<ModalDialog> Dialogs { get; } = new(8);

    [Parameter]
    public bool IsBackdrop { get; set; }

    [Parameter]
    public bool IsKeyboard { get; set; } = true;

    [Parameter]
    public bool IsFade { get; set; } = true;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Modal, Task>? FirstAfterRenderCallbackAsync { get; set; }

    [Parameter]
    public Func<Task>? OnShownAsync { get; set; }

    [Parameter]
    public Func<Task>? OnCloseAsync { get; set; }

    private string? Backdrop => IsBackdrop ? null : "static";

    private string? KeyboardString => IsKeyboard ? "true" : "false";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && FirstAfterRenderCallbackAsync != null)
        {
            await FirstAfterRenderCallbackAsync(this);
        }
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, nameof(ShownCallback), nameof(CloseCallback));

    internal void AddDialog(ModalDialog dialog)
    {
        Dialogs.Add(dialog);
        ResetShownDialog(dialog);
    }

    internal void RemoveDialog(ModalDialog dialog)
    {
        Dialogs.Remove(dialog);

        if (Dialogs.Any())
        {
            ResetShownDialog(Dialogs.Last());
        }
    }

    private void ResetShownDialog(ModalDialog dialog)
    {
        Dialogs.ForEach(d =>
        {
            d.IsShown = d == dialog;
        });
    }

    [JSInvokable]
    public async Task ShownCallback()
    {
        if (OnShownAsync != null)
        {
            await OnShownAsync();
        }
    }

    [JSInvokable]
    public async Task CloseCallback()
    {
        var dialog = Dialogs.FirstOrDefault(d => d.IsShown);
        if (dialog != null)
        {
            Dialogs.Remove(dialog);
        }

        if (Dialogs.Any())
        {
            ResetShownDialog(Dialogs.Last());
        }

        if (OnCloseAsync != null)
        {
            await OnCloseAsync();
        }
    }

    public Task Toggle() => InvokeVoidAsync("execute", Id, "toggle");

    public Task Show() => InvokeVoidAsync("execute", Id, "show");

    public Task Close() => InvokeVoidAsync("execute", Id, "hide");

    public void SetHeaderText(string text)
    {
        var dialog = Dialogs.FirstOrDefault(d => d.IsShown);
        dialog?.SetHeaderText(text);
    }
}
