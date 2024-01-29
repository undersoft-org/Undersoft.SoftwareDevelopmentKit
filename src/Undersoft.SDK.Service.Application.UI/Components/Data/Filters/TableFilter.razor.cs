using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("table-filter")]
public partial class TableFilter : IFilter
{
    private string? FilterClassString => CssBuilder.Default(Icon)
        .AddClass("active", IsActive)
        .Build();

    private string? ClassString => CssBuilder.Default("filter-icon")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public string? PlusIcon { get; set; }

    [Parameter]
    public string? MinusIcon { get; set; }

    [Parameter]
    public string? NotSupportedMessage { get; set; }

    [NotNull]
    private string? Title { get; set; }

    [NotNull]
    internal string? FieldKey { get; set; }

    private int Count { get; set; }

    public bool ShowMoreButton { get; set; } = true;

    [NotNull]
    public IFilterAction? FilterAction { get; set; }

    internal bool HasFilter => (Table != null) && Table.Filters.ContainsKey(Column.GetFieldName());

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public ITableColumn? Column { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    [Parameter]
    public bool IsHeaderRow { get; set; }

    [Parameter]
    [NotNull]
    public string? FilterButtonText { get; set; }

    [Parameter]
    public ITable? Table { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TableFilter>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? Step => Column.Step?.ToString() ?? "0.01";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Title = Column.GetDisplayName();
        FieldKey = Column.GetFieldName();
        Column.Filter = this;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        FilterButtonText ??= Localizer[nameof(FilterButtonText)];
        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        NotSupportedMessage ??= Localizer[nameof(NotSupportedMessage)];

        PlusIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableFilterPlusIcon);
        MinusIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableFilterMinusIcon);
    }

    protected override async Task ModuleInitAsync()
    {
        if (!IsHeaderRow)
        {
            await base.ModuleInitAsync();
        }
    }

    private async Task OnClickReset()
    {
        Count = 0;

        if (Table != null)
        {
            Table.Filters.Remove(FieldKey);
            FilterAction.Reset();
            await Table.OnFilterAsync();
        }
    }

    private async Task OnClickConfirm()
    {
        await OnFilterAsync();
    }

    internal async Task OnFilterAsync()
    {
        if (Table != null)
        {
            if (FilterAction.GetFilterConditions().Any())
            {
                Table.Filters[FieldKey] = FilterAction;
            }
            else
            {
                Table.Filters.Remove(FieldKey);
            }
            await Table.OnFilterAsync();
        }
    }

    private void OnClickPlus()
    {
        if (Count == 0)
        {
            Count++;
        }
    }

    private void OnClickMinus()
    {
        if (Count == 1)
        {
            Count--;
        }
    }
}
