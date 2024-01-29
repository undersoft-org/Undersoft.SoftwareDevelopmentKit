using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Transfer<TValue>
{
    [Inject]
    [NotNull]
    private IStringLocalizer<Transfer<TValue>>? Localizer { get; set; }

    private string? LeftButtonClassName => CssBuilder.Default()
        .AddClass("d-none", string.IsNullOrEmpty(LeftButtonText))
        .Build();

    private string? RightButtonClassName => CssBuilder.Default("me-1")
        .AddClass("d-none", string.IsNullOrEmpty(RightButtonText))
        .Build();

    private string? ValidateClass => CssBuilder.Default()
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private List<SelectedItem> LeftItems { get; } = new List<SelectedItem>();

    private List<SelectedItem> RightItems { get; } = new List<SelectedItem>();

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public IEnumerable<SelectedItem>? Items { get; set; }

    [Parameter]
    public Func<IEnumerable<SelectedItem>, Task>? OnSelectedItemsChanged { get; set; }

    [Parameter]
    public string? LeftPanelText { get; set; }

    [Parameter]
    public string? RightPanelText { get; set; }

    [Parameter]
    public string? LeftIcon { get; set; }

    [Parameter]
    public string? RightIcon { get; set; }

    [Parameter]
    public string? LeftButtonText { get; set; }

    [Parameter]
    public string? RightButtonText { get; set; }

    [Parameter]
    public bool ShowSearch { get; set; }

    [Parameter]
    public string? LeftPannelSearchPlaceHolderString { get; set; }

    [Parameter]
    public string? RightPannelSearchPlaceHolderString { get; set; }

    [Parameter]
    public int Max { get; set; }

    [Parameter]
    [NotNull]
    public string? MaxErrorMessage { get; set; }

    [Parameter]
    public int Min { get; set; }

    [Parameter]
    [NotNull]
    public string? MinErrorMessage { get; set; }

    [Parameter]
    [NotNull]
    public Func<SelectedItem, string?>? OnSetItemClass { get; set; }

    [Parameter]
    public RenderFragment<List<SelectedItem>>? LeftHeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem>? LeftItemTemplate { get; set; }

    [Parameter]
    public RenderFragment<List<SelectedItem>>? RightHeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem>? RightItemTemplate { get; set; }

    [Inject]
    [NotNull]
    public IStringLocalizerFactory? LocalizerFactory { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (OnSetItemClass == null)
        {
            OnSetItemClass = _ => null;
        }

        if (EditContext != null && FieldIdentifier != null)
        {
            var pi = FieldIdentifier.Value.Model.GetType().GetPropertyByName(FieldIdentifier.Value.FieldName);
            if (pi != null)
            {
                var required = pi.GetCustomAttribute<RequiredAttribute>(true);
                if (required != null)
                {
                    Rules.Add(new RequiredValidator() { LocalizerFactory = LocalizerFactory, ErrorMessage = required.ErrorMessage, AllowEmptyString = required.AllowEmptyStrings });
                }
            }
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        LeftPanelText ??= Localizer[nameof(LeftPanelText)];
        RightPanelText ??= Localizer[nameof(RightPanelText)];
        MinErrorMessage ??= Localizer[nameof(MinErrorMessage)];
        MaxErrorMessage ??= Localizer[nameof(MaxErrorMessage)];

        LeftIcon ??= IconTheme.GetIconByKey(ComponentIcons.TransferLeftIcon);
        RightIcon ??= IconTheme.GetIconByKey(ComponentIcons.TransferRightIcon);

        var list = CurrentValueAsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        LeftItems.Clear();
        RightItems.Clear();

        Items ??= Enumerable.Empty<SelectedItem>();

        LeftItems.AddRange(Items);
        LeftItems.RemoveAll(i => list.Any(l => l == i.Value));

        foreach (var t in list)
        {
            var item = Items.FirstOrDefault(i => i.Value == t);
            if (item != null)
            {
                RightItems.Add(item);
            }
        }

        ResetRules();
    }

    private int _min;
    private int _max;
    private void ResetRules()
    {
        if (Max != _max)
        {
            _max = Max;
            Rules.RemoveAll(v => v is MaxValidator);

            if (Max > 0)
            {
                Rules.Add(new MaxValidator() { Value = Max, ErrorMessage = MaxErrorMessage });
            }
        }

        if (Min != _min)
        {
            _min = Min;
            Rules.RemoveAll(v => v is MinValidator);

            if (Min > 0)
            {
                Rules.Add(new MinValidator() { Value = Min, ErrorMessage = MinErrorMessage });
            }
        }
    }

    private async Task TransferItems(List<SelectedItem> source, List<SelectedItem> target, bool isLeft)
    {
        if (Items != null)
        {
            var items = source.Where(i => i.Active).ToList();
            items.ForEach(i => i.Active = false);

            source.RemoveAll(i => items.Contains(i));
            target.AddRange(items);

            if (isLeft)
            {
                CurrentValueAsString = string.Join(",", target.Select(i => i.Value));
            }
            else
            {
                CurrentValueAsString = string.Join(",", source.Select(i => i.Value));
            }
            if (OnSelectedItemsChanged != null)
            {
                await OnSelectedItemsChanged(isLeft ? target : source);
            }

            if (ValidateForm == null && (Min > 0 || Max > 0))
            {
                var validationContext = new ValidationContext(Value);
                if (FieldIdentifier.HasValue)
                {
                    validationContext.MemberName = FieldIdentifier.Value.FieldName;
                }
                var validationResults = new List<ValidationResult>();

                await ValidatePropertyAsync(RightItems, validationContext, validationResults);
                ToggleMessage(validationResults, true);
            }
        }
    }

    protected override bool TryParseValueFromString(string value, out TValue result, out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        if (typeof(TValue) == typeof(string))
        {
            result = (TValue)(object)value;
        }
        else if (typeof(IEnumerable<string>).IsAssignableFrom(typeof(TValue)))
        {
            var v = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            result = (TValue)(object)new List<string>(v);
        }
        else if (typeof(IEnumerable<SelectedItem>).IsAssignableFrom(typeof(TValue)))
        {
            result = (TValue)(object)RightItems.Select(i => new SelectedItem(i.Value, i.Text)).ToList();
        }
        else
        {
            result = default!;
        }
        return true;
    }

    protected override string? FormatValueAsString(TValue value) => value == null
        ? null
        : Utility.ConvertValueToString(value);

    private Task SelectedItemsChanged()
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    private static bool GetButtonState(IEnumerable<SelectedItem> source) => !(source.Any(i => i.Active));
}
