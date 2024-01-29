using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class EditDialog<TModel>
{
    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public Func<EditContext, Task>? OnSaveAsync { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    [Parameter]
    public bool ShowLoading { get; set; }

    [Parameter]
    public bool IsTracking { get; set; }

    [Parameter]
    public ItemChangedType ItemChangedType { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    [Parameter]
    public Func<Task>? OnCloseAsync { get; set; }

    [Parameter]
    public bool? DisableAutoSubmitFormByEnter { get; set; }

    [Parameter]
    public RenderFragment<TModel>? FooterTemplate { get; set; }

    [Parameter]
    public string? SaveButtonIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<EditDialog<TModel>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SaveButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.DialogSaveButtonIcon);

        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        SaveButtonText ??= Localizer[nameof(SaveButtonText)];
    }

    private async Task OnValidSubmitAsync(EditContext context)
    {
        if (OnSaveAsync != null)
        {
            await ToggleLoading(true);
            await OnSaveAsync(context);
            await ToggleLoading(false);
        }
    }

    public async ValueTask ToggleLoading(bool state)
    {
        if (ShowLoading)
        {
            await InvokeVoidAsync("execute", Id, state);
        }
    }

    private RenderFragment RenderFooter => builder =>
    {
        if (FooterTemplate != null)
        {
            builder.AddContent(1, FooterTemplate(Model));
        }
        else
        {
            if (!IsTracking)
            {
                builder.OpenComponent<DialogCloseButton>(20);
                builder.AddAttribute(21, nameof(Button.Text), CloseButtonText);
                builder.AddAttribute(22, nameof(Button.OnClickWithoutRender), OnCloseAsync);
                builder.CloseComponent();
            }
            builder.OpenComponent<Button>(30);
            builder.AddAttribute(31, nameof(Button.Color), Color.Primary);
            builder.AddAttribute(32, nameof(Button.Icon), SaveButtonIcon);
            builder.AddAttribute(33, nameof(Button.Text), SaveButtonText);
            builder.AddAttribute(34, nameof(Button.ButtonType), ButtonType.Submit);
            builder.CloseComponent();
        }
    };
}
