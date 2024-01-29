using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleNotInherited]
public class TableToolbarPopconfirmButton<TItem> : PopConfirmButtonBase, ITableToolbarButton<TItem>
{
    [Parameter]
    public Func<IEnumerable<TItem>, Task>? OnConfirmCallback { get; set; }

    [Parameter]
    public bool IsShow { get; set; } = true;

    [Parameter]
    public bool IsEnableWhenSelectedOneRow { get; set; }

    [Parameter]
    public Func<IEnumerable<TItem>, bool>? IsDisabledCallback { get; set; }

    [CascadingParameter]
    protected TableToolbar<TItem>? Toolbar { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<PopConfirmButton>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Toolbar?.AddButton(this);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];
        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        Content ??= Localizer[nameof(Content)];
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        Toolbar?.RemoveButton(this);
        await base.DisposeAsync(disposing);
    }
}
