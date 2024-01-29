using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Search
{
    [NotNull]
    private string? ButtonIcon { get; set; }

    [Parameter]
    public bool ShowClearButton { get; set; }

    [Parameter]
    public string? ClearButtonIcon { get; set; }

    [Parameter]
    public string? ClearButtonText { get; set; }

    [Parameter]
    public Color ClearButtonColor { get; set; } = Color.Secondary;

    [Parameter]
    public Color SearchButtonColor { get; set; } = Color.Primary;

    [Parameter]
    public string? SearchButtonIcon { get; set; }

    [Parameter]
    public string? SearchButtonLoadingIcon { get; set; }

    [Parameter]
    public bool IsAutoClearAfterSearch { get; set; }

    [Parameter]
    public bool IsOnInputTrigger { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchButtonText { get; set; }

    [Parameter]
    public Func<string, Task>? OnSearch { get; set; }

    [Parameter]
    public Func<string, Task>? OnClear { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Search>? Localizer { get; set; }

    private JSInterop<Search>? Interop { get; set; }

    protected override string? ClassString => CssBuilder.Default("search")
        .AddClassFromAttributes(AdditionalAttributes)
        .AddClass(base.ClassString)
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SearchButtonText ??= Localizer[nameof(SearchButtonText)];

        SkipEnter = true;
        SkipEsc = true;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ClearButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.SearchClearButtonIcon);
        SearchButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.SearchButtonIcon);
        SearchButtonLoadingIcon ??= IconTheme.GetIconByKey(ComponentIcons.SearchButtonLoadingIcon);

        ButtonIcon = SearchButtonIcon;
    }

    protected async Task OnSearchClick()
    {
        if (OnSearch != null)
        {
            ButtonIcon = SearchButtonLoadingIcon;
            await OnSearch(CurrentValueAsString);
            ButtonIcon = SearchButtonIcon;
        }

        if (IsAutoClearAfterSearch)
        {
            CurrentValueAsString = "";
        }

        await FocusAsync();
    }

    protected async Task OnClearClick()
    {
        if (OnClear != null)
        {
            await OnClear(CurrentValueAsString);
        }
        CurrentValueAsString = "";
    }

    protected override async Task OnKeyUp(KeyboardEventArgs args)
    {
        await base.OnKeyUp(args);

        if (!string.IsNullOrEmpty(CurrentValueAsString))
        {
            if (args.Key == "Escape")
            {
                if (OnEscAsync != null)
                {
                    await OnEscAsync(Value);
                }

                await OnClearClick();
            }

            if (IsOnInputTrigger || args.Key == "Enter")
            {
                if (OnEnterAsync != null)
                {
                    await OnEnterAsync(Value);
                }

                await OnSearchClick();
            }
        }
    }

    protected override async Task OnClickItem(string item)
    {
        await base.OnClickItem(item);

        if (IsOnInputTrigger)
        {
            await OnSearchClick();
        }
    }

    protected override async Task RegisterComposition()
    {
        if (ValidateForm != null)
        {
            Interop ??= new JSInterop<Search>(JSRuntime);

            await Interop.InvokeVoidAsync(this, FocusElement, "bb_composition", nameof(TriggerOnChange));
        }
    }

    protected override ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            Interop?.Dispose();
        }

        return base.DisposeAsync(disposing);
    }
}
