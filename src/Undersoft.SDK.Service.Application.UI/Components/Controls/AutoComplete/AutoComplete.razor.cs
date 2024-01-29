using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public partial class AutoComplete
{

    private bool IsLoading { get; set; }

    private bool IsShown { get; set; }

    protected virtual string? ClassString => CssBuilder.Default("auto-complete")
        .AddClass("is-loading", IsLoading)
        .Build();

    protected string? DropdownMenuClassString => CssBuilder.Default("dropdown-menu")
        .AddClass("show", IsShown)
        .Build();

    [NotNull]
    protected List<string>? FilterItems { get; private set; }

    [Parameter]
    [NotNull]
    public IEnumerable<string>? Items { get; set; }

    [Parameter]
    [NotNull]
    public string? NoDataTip { get; set; }

    [Parameter]
    [NotNull]
    public int? DisplayCount { get; set; }

    [Parameter]
    public bool IsLikeMatch { get; set; } = false;

    [Parameter]
    public bool IgnoreCase { get; set; } = true;

    [Parameter]
    public Func<string, Task<IEnumerable<string>>>? OnCustomFilter { get; set; }

    [Parameter]
    public Func<string, Task>? OnSelectedItemChanged { get; set; }

    [Parameter]
    public int Debounce { get; set; }

    [Parameter]
    public bool SkipEnter { get; set; }

    [Parameter]
    public bool SkipEsc { get; set; }

    [Parameter]
    public bool ShowDropdownListOnFocus { get; set; } = true;

    [Parameter]
    public RenderFragment<string>? ItemTemplate { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<AutoComplete>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private JSInterop<AutoComplete>? Interop { get; set; }

    private string CurrentSelectedItem { get; set; } = "";

    protected ElementReference AutoCompleteElement { get; set; }

    protected int? CurrentItemIndex { get; set; }

    private string? IconString => CssBuilder.Default("ac-loading")
        .AddClass(Icon, !string.IsNullOrEmpty(Icon))
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        NoDataTip ??= Localizer[nameof(NoDataTip)];
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        Items ??= Enumerable.Empty<string>();
        FilterItems ??= new List<string>();

        SkipRegisterEnterEscJSInvoke = true;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Icon ??= IconTheme.GetIconByKey(ComponentIcons.AutoCompleteIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (CurrentItemIndex.HasValue)
        {
            await JSRuntime.InvokeVoidAsync(AutoCompleteElement, "bb_autoScrollItem", CurrentItemIndex.Value);
        }

        if (firstRender)
        {
            await RegisterComposition();

            if (Debounce > 0)
            {
                await JSRuntime.InvokeVoidAsync(FocusElement, "bb_setDebounce", Debounce);
            }
        }
    }

    protected void OnBlur()
    {
        CurrentSelectedItem = "";
        IsShown = false;
    }

    protected virtual async Task OnClickItem(string val)
    {
        CurrentValue = val;
        if (OnSelectedItemChanged != null)
        {
            await OnSelectedItemChanged(val);
        }
    }

    protected virtual async Task OnFocus(FocusEventArgs args)
    {
        if (ShowDropdownListOnFocus)
        {
            await OnKeyUp(new KeyboardEventArgs());
        }
    }

    protected virtual async Task OnKeyUp(KeyboardEventArgs args)
    {
        if (!IsLoading)
        {
            IsLoading = true;
            if (OnCustomFilter != null)
            {
                var items = await OnCustomFilter(CurrentValueAsString);
                FilterItems = items.ToList();
            }
            else
            {
                var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                var items = IsLikeMatch ?
                    Items.Where(s => s.Contains(CurrentValueAsString, comparison)) :
                    Items.Where(s => s.StartsWith(CurrentValueAsString, comparison));
                FilterItems = DisplayCount == null ? items.ToList() : items.Take(DisplayCount.Value).ToList();
            }
            IsLoading = false;
        }

        IsShown = true;

        var source = FilterItems;
        if (source.Any())
        {
            if (args.Key == "ArrowUp")
            {
                var index = source.IndexOf(CurrentSelectedItem) - 1;
                if (index < 0)
                {
                    index = source.Count - 1;
                }
                CurrentSelectedItem = source[index];
                CurrentItemIndex = index;
            }
            else if (args.Key == "ArrowDown")
            {
                var index = source.IndexOf(CurrentSelectedItem) + 1;
                if (index > source.Count - 1)
                {
                    index = 0;
                }
                CurrentSelectedItem = source[index];
                CurrentItemIndex = index;
            }
            else if (args.Key == "Escape")
            {
                OnBlur();
                if (!SkipEsc && OnEscAsync != null)
                {
                    await OnEscAsync(Value);
                }
            }
            else if (args.Key == "Enter")
            {
                if (!string.IsNullOrEmpty(CurrentSelectedItem))
                {
                    CurrentValueAsString = CurrentSelectedItem;
                    if (OnSelectedItemChanged != null)
                    {
                        await OnSelectedItemChanged(CurrentSelectedItem);
                    }
                }

                OnBlur();
                if (!SkipEnter && OnEnterAsync != null)
                {
                    await OnEnterAsync(Value);
                }
            }
        }
    }

    [JSInvokable]
    public void TriggerOnChange(string val)
    {
        CurrentValueAsString = val;
    }

    protected virtual async Task RegisterComposition()
    {
        if (ValidateForm != null)
        {
            Interop ??= new JSInterop<AutoComplete>(JSRuntime);
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
