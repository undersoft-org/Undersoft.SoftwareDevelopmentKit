namespace Undersoft.SDK.Service.Application.Components;

public partial class Message : IDisposable
{
    private string? ClassString => CssBuilder.Default("message")
        .AddClass("is-bottom", Placement != Placement.Top)
        .Build();

    private string? StyleName => CssBuilder.Default()
        .AddClass("top: 1rem;", Placement != Placement.Bottom)
        .AddClass("bottom: 1rem;", Placement == Placement.Bottom)
        .Build();

    private List<MessageOption> Messages { get; } = new List<MessageOption>();

    [Parameter]
    public Placement Placement { get; set; } = Placement.Top;

    [Inject]
    [NotNull]
    public MessageService? MessageService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        MessageService.Register(this, Show);
    }

    public void SetPlacement(Placement placement)
    {
        Placement = placement;
        StateHasChanged();
    }

    protected async Task Show(MessageOption option)
    {
        Messages.Add(option);
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task Clear()
    {
        Messages.Clear();
        await InvokeAsync(StateHasChanged);
    }

    private static async Task OnDismiss(MessageOption option)
    {
        if (option.OnDismiss != null)
        {
            await option.OnDismiss();
        }
    }

    private List<MessageOption> GetMessages()
    {
        if (Placement != Placement.Top)
        {
            Messages.Reverse();
        }

        return Messages;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            MessageService.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
