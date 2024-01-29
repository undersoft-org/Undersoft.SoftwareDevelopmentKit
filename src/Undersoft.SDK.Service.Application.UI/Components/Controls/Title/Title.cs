namespace Undersoft.SDK.Service.Application.Components;

public class Title : Component, IDisposable
{
    [Inject]
    [NotNull]
    private TitleService? TitleService { get; set; }

    [Parameter]
    public string? Text { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(Text))
        {
            TitleService.Register(this, SetTitle);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!string.IsNullOrEmpty(Text))
        {
            await SetTitle(Text);
        }
    }

    private ValueTask SetTitle(string title) => JSRuntime.InvokeVoidAsync(identifier: "$.bb_setTitle", title);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TitleService.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
