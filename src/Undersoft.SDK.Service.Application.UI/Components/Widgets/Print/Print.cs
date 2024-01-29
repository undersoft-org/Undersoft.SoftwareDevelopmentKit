namespace Undersoft.SDK.Service.Application.Components;

public class Print : Component, IDisposable
{
    [Inject]
    [NotNull]
    private PrintService? PrintService { get; set; }

    [Inject]
    [NotNull]
    private DialogService? DialogService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        PrintService.Register(this, PrintDialogAsync);
    }

    private Task PrintDialogAsync(DialogOption option) => DialogService.Show(option);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            PrintService.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
