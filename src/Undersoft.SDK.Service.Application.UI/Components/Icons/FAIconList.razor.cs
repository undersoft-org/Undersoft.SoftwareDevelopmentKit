using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Undersoft.SDK.Service.Application;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/BootstrapBlazor.FontAwesome/Components/FAIconList.razor.js", JSObjectReference = true, Relative = false)]
public partial class FAIconList
{
    private string? ClassString => CssBuilder.Default("icon-vector")
        .AddClass("is-catalog", ShowCatalog)
        .AddClass("is-dialog", ShowCopyDialog)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public bool ShowCopyDialog { get; set; }

    [Parameter]
    public bool ShowCatalog { get; set; }

    [Parameter]
    [NotNull]
    public string? DialogHeaderText { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public EventCallback<string?> IconChanged { get; set; }

    [Parameter]
    public bool IsCopy { get; set; }

    [Parameter]
    public string? CopiedTooltipText { get; set; }

    [Inject]
    [NotNull]
    private DialogService? DialogService { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<IconDialog>? Localizer { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DialogHeaderText ??= Localizer[nameof(DialogHeaderText)];
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, nameof(UpdateIcon), nameof(ShowDialog), IsCopy);

    [JSInvokable]
    public async Task UpdateIcon(string icon)
    {
        Icon = icon;
        if (IconChanged.HasDelegate)
        {
            await IconChanged.InvokeAsync(Icon);
        }
        else
        {
            StateHasChanged();
        }
    }

    [JSInvokable]
    public Task ShowDialog(string text) => DialogService.ShowCloseDialog<IconDialog>(DialogHeaderText, parameters =>
    {
        parameters.Add(nameof(IconDialog.IconName), text);
    });
}
