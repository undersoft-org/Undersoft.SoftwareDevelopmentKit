namespace Undersoft.SDK.Service.Application.Components;

public partial class SweetAlert : IAsyncDisposable
{
    [NotNull]
    private Modal? ModalContainer { get; set; }

    [Inject]
    [NotNull]
    private SwalService? SwalService { get; set; }

    private bool IsShowDialog { get; set; }

    private bool IsAutoHide { get; set; }

    private int Delay { get; set; }

    private CancellationTokenSource DelayToken { get; set; } = new();

    [NotNull]
    private Dictionary<string, object>? DialogParameter { get; set; }

    [NotNull]
    private Func<Task>? OnCloseAsync { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SwalService.Register(this, Show);

        OnCloseAsync = () =>
        {
            IsShowDialog = false;
            DialogParameter = null;
            if (AutoHideCheck())
            {
                DelayToken.Cancel();
            }
            StateHasChanged();
            return Task.CompletedTask;
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (IsShowDialog)
        {
            await ModalContainer.Show();

            if (AutoHideCheck())
            {
                try
                {
                    if (DelayToken.IsCancellationRequested)
                    {
                        DelayToken = new();
                    }
                    await Task.Delay(Delay, DelayToken.Token);
                    await ModalContainer.Close();
                }
                catch (TaskCanceledException) { }
            }
        }
    }

    private bool AutoHideCheck() => IsAutoHide && Delay > 0;

    private Task Show(SwalOption option)
    {
        if (!IsShowDialog)
        {
            IsShowDialog = true;

            IsAutoHide = option.IsAutoHide;
            Delay = option.Delay;

            option.Modal = ModalContainer;
            var parameters = option.ToAttributes();
            parameters.Add(
                nameof(ModalDialog.BodyTemplate),
                BootstrapDynamicComponent.CreateComponent<SweetAlertBody>(option.Parse()).Render()
            );

            DialogParameter = parameters;

            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    private RenderFragment RenderDialog() =>
        builder =>
        {
            if (DialogParameter != null)
            {
                var index = 0;
                builder.OpenComponent<ModalDialog>(index++);
                builder.SetKey(DialogParameter);
                builder.AddMultipleAttributes(index++, DialogParameter);
                builder.CloseComponent();
            }
        };

    private bool disposed;

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposed && disposing)
        {
            disposed = true;

            if (IsShowDialog)
            {
                DelayToken.Cancel();
                await ModalContainer.Close();
                IsShowDialog = false;
            }

            DelayToken.Dispose();

            SwalService.UnRegister(this);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
