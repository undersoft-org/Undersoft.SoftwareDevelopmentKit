using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public partial class AutoFill<TValue>
{
    private bool _isLoading;
    private bool _isShown;

    protected virtual string? ClassString => CssBuilder.Default("auto-complete auto-fill")
        .AddClass("is-loading", _isLoading)
        .Build();

    protected string? DropdownMenuClassString => CssBuilder.Default("dropdown-menu")
        .AddClass("show", _isShown)
        .Build();

    [NotNull]
    protected List<TValue>? FilterItems { get; private set; }

    [Parameter]
    [NotNull]
    public IEnumerable<TValue>? Items { get; set; }

    [Parameter]
    [NotNull]
    public string? NoDataTip { get; set; }

    [Parameter]
    [NotNull]
    public int? DisplayCount { get; set; }

    [Parameter]
    public bool IsLikeMatch { get; set; }

    [Parameter]
    public bool IgnoreCase { get; set; } = true;

    [Parameter]
    public Func<string, Task<IEnumerable<TValue>>>? OnCustomFilter { get; set; }

    [Parameter]
    public RenderFragment<TValue>? Template { get; set; }

    [Parameter]
    [NotNull]
    public Func<TValue, string>? OnGetDisplayText { get; set; }

    [Parameter]
    public bool SkipEnter { get; set; }

    [Parameter]
    public bool SkipEsc { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnSelectedItemChanged { get; set; }

    [Parameter]
    public int Debounce { get; set; }

    [Parameter]
    public bool ShowDropdownListOnFocus { get; set; } = true;

    [Parameter]
    public string? Icon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<AutoComplete>? Localizer { get; set; }

    private string InputString { get; set; } = "";

    private TValue? ActiveSelectedItem { get; set; }

    private JSInterop<AutoFill<TValue>>? Interop { get; set; }

    private ElementReference AutoFillElement { get; set; }

    private int? CurrentItemIndex { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        NoDataTip ??= Localizer[nameof(NoDataTip)];
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        Items ??= Enumerable.Empty<TValue>();
        FilterItems ??= new List<TValue>();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.AutoFillIcon);

        OnGetDisplayText ??= v => v?.ToString() ?? "";
        InputString = OnGetDisplayText(Value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (CurrentItemIndex.HasValue)
        {
            await JSRuntime.InvokeVoidAsync(AutoFillElement, "bb_autoScrollItem", CurrentItemIndex.Value);
        }

        if (firstRender)
        {
            if (ValidateForm != null)
            {
                Interop ??= new JSInterop<AutoFill<TValue>>(JSRuntime);

                await Interop.InvokeVoidAsync(this, FocusElement, "bb_composition", nameof(TriggerOnChange));
            }

            if (Debounce > 0)
            {
                await JSRuntime.InvokeVoidAsync(AutoFillElement, "bb_setDebounce", Debounce);
            }
        }
    }

    protected async Task OnBlur()
    {
        _isShown = false;
        if (OnSelectedItemChanged != null && ActiveSelectedItem != null)
        {
            await OnSelectedItemChanged(ActiveSelectedItem);
            ActiveSelectedItem = default;
        }
    }

    protected virtual async Task OnClickItem(TValue val)
    {
        CurrentValue = val;
        InputString = OnGetDisplayText(val);
        ActiveSelectedItem = default;
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
        if (!_isLoading)
        {
            _isLoading = true;
            if (OnCustomFilter != null)
            {
                var items = await OnCustomFilter(InputString);
                FilterItems = items.ToList();
            }
            else
            {
                var items = FindItem();
                FilterItems = DisplayCount == null ? items.ToList() : items.Take(DisplayCount.Value).ToList();
            }
            _isLoading = false;
        }

        var source = FilterItems;
        if (source.Any())
        {
            _isShown = true;
            if (args.Key == "ArrowUp")
            {
                var index = 0;
                if (ActiveSelectedItem != null)
                {
                    index = source.IndexOf(ActiveSelectedItem) - 1;
                    if (index < 0)
                    {
                        index = source.Count - 1;
                    }
                }
                ActiveSelectedItem = source[index];
                CurrentItemIndex = index;
            }
            else if (args.Key == "ArrowDown")
            {
                var index = 0;
                if (ActiveSelectedItem != null)
                {
                    index = source.IndexOf(ActiveSelectedItem) + 1;
                    if (index > source.Count - 1)
                    {
                        index = 0;
                    }
                }
                ActiveSelectedItem = source[index];
                CurrentItemIndex = index;
            }
            else if (args.Key == "Escape")
            {
                await OnBlur();
                if (!SkipEsc && OnEscAsync != null)
                {
                    await OnEscAsync(Value);
                }
            }
            else if (args.Key == "Enter")
            {
                if (ActiveSelectedItem == null)
                {
                    ActiveSelectedItem = FindItem().FirstOrDefault();
                }
                if (ActiveSelectedItem != null)
                {
                    InputString = OnGetDisplayText(ActiveSelectedItem);
                }
                await OnBlur();
                if (!SkipEnter && OnEnterAsync != null)
                {
                    await OnEnterAsync(Value);
                }
            }
        }

        IEnumerable<TValue> FindItem()
        {
            var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return IsLikeMatch ?
                Items.Where(s => OnGetDisplayText(s).Contains(InputString, comparison)) :
                Items.Where(s => OnGetDisplayText(s).StartsWith(InputString, comparison));
        }
    }

    [JSInvokable]
    public void TriggerOnChange(string val)
    {
        InputString = val;
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
