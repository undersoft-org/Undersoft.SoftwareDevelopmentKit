namespace Undersoft.SDK.Service.Application.Components;

public partial class Dialog : IDisposable
{
    [NotNull]
    private Modal? ModalContainer { get; set; }

    private Dictionary<Dictionary<string, object>, (bool IsKeyboard, bool IsBackdrop)> DialogParameters { get; } = new();

    private bool IsKeyboard { get; set; }

    private bool IsBackdrop { get; set; }

    [Inject]
    [NotNull]
    private DialogService? DialogService { get; set; }

    [NotNull]
    private Func<Task>? OnShownAsync { get; set; }

    [NotNull]
    private Func<Task>? OnCloseAsync { get; set; }

    private Dictionary<string, object>? CurrentParameter { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DialogService.Register(this, Show);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (CurrentParameter != null)
        {
            await ModalContainer.Show();
        }
    }

    private Task Show(DialogOption option)
    {
        OnShownAsync = async () =>
        {
            if (option.OnShownAsync != null)
            {
                await option.OnShownAsync();
            }
        };

        OnCloseAsync = async () =>
        {
            if (option.OnCloseAsync != null)
            {
                await option.OnCloseAsync();
            }

            if (CurrentParameter != null)
            {
                DialogParameters.Remove(CurrentParameter);

                var p = DialogParameters.LastOrDefault();
                CurrentParameter = p.Key;
                IsKeyboard = p.Value.IsKeyboard;
                IsBackdrop = p.Value.IsBackdrop;

                StateHasChanged();
            }
        };

        IsKeyboard = option.IsKeyboard;
        IsBackdrop = option.IsBackdrop;

        option.Modal = ModalContainer;

#pragma warning disable CS0618      
        option.Dialog = ModalContainer;
#pragma warning restore CS0618      

        var parameters = option.ToAttributes();
        var content = option.BodyTemplate ?? option.Component?.Render();
        if (content != null)
        {
            parameters.Add(nameof(ModalDialog.BodyTemplate), content);
        }

        if (option.HeaderTemplate != null)
        {
            parameters.Add(nameof(ModalDialog.HeaderTemplate), option.HeaderTemplate);
        }

        if (option.HeaderToolbarTemplate != null)
        {
            parameters.Add(nameof(ModalDialog.HeaderToolbarTemplate), option.HeaderToolbarTemplate);
        }

        if (option.FooterTemplate != null)
        {
            parameters.Add(nameof(ModalDialog.FooterTemplate), option.FooterTemplate);
        }

        if (!string.IsNullOrEmpty(option.Class))
        {
            parameters.Add(nameof(ModalDialog.Class), option.Class);
        }

        if (option.OnSaveAsync != null)
        {
            parameters.Add(nameof(ModalDialog.OnSaveAsync), option.OnSaveAsync);
        }

        if (option.CloseButtonText != null)
        {
            parameters.Add(nameof(ModalDialog.CloseButtonText), option.CloseButtonText);
        }

        if (option.SaveButtonText != null)
        {
            parameters.Add(nameof(ModalDialog.SaveButtonText), option.SaveButtonText);
        }

        CurrentParameter = parameters;

        DialogParameters.Add(parameters, (IsKeyboard, IsKeyboard));
        StateHasChanged();
        return Task.CompletedTask;
    }

    private static RenderFragment RenderDialog(int index, IEnumerable<KeyValuePair<string, object>> parameter) => builder =>
    {
        builder.OpenComponent<ModalDialog>(100 + index);
        builder.AddMultipleAttributes(101 + index, parameter);
        builder.SetKey(parameter);
        builder.CloseComponent();
    };

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DialogService.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
