using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Calendar
{
    [NotNull]
    private string? PreviousYear { get; set; }

    [NotNull]
    private string? NextYear { get; set; }

    [NotNull]
    private string? PreviousMonth { get; set; }

    [NotNull]
    private string? NextMonth { get; set; }

    [NotNull]
    private string? Today { get; set; }

    [NotNull]
    private string? PreviousWeek { get; set; }

    [NotNull]
    private string? NextWeek { get; set; }

    [NotNull]
    private string? WeekText { get; set; }

    [NotNull]
    private List<string>? WeekLists { get; set; }

    [NotNull]
    private string? WeekHeaderText { get; set; }

    [NotNull]
    private List<string>? Months { get; set; }

    private string? GetTitle() => Localizer["Title", Value.Year, Months.ElementAt(Value.Month - 1)];

    [NotNull]
    private string? WeekNumberText { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Calendar>? Localizer { get; set; }

    private string GetWeekDayString(int offset) => $"{Value.AddDays(offset - (int)Value.DayOfWeek).Day}";

    private string? GetWeekDayClassString(int offset) => CssBuilder.Default("week-header")
            .AddClass("is-today", Value.AddDays(offset - (int)Value.DayOfWeek) == DateTime.Today)
            .Build();

    private static string? GetCalendarCellString(CalendarCellValue item) => CssBuilder.Default()
            .AddClass("prev", item.CellValue.Month < item.CalendarValue.Month)
            .AddClass("next", item.CellValue.Month > item.CalendarValue.Month)
            .AddClass("current", item.CellValue.Month == item.CalendarValue.Month)
            .AddClass("is-selected", item.CellValue.Ticks == item.CalendarValue.Ticks)
            .AddClass("is-today", item.CellValue.Ticks == DateTime.Today.Ticks)
            .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Value == DateTime.MinValue)
        {
            Value = DateTime.Today;
        }

        PreviousYear = Localizer[nameof(PreviousYear)];
        NextYear = Localizer[nameof(NextYear)];
        PreviousMonth = Localizer[nameof(PreviousMonth)];
        NextMonth = Localizer[nameof(NextMonth)];
        Today = Localizer[nameof(Today)];
        WeekLists = Localizer[nameof(WeekLists)].Value.Split(',').ToList();
        PreviousWeek = Localizer[nameof(PreviousWeek)];
        NextWeek = Localizer[nameof(NextWeek)];
        WeekText = Localizer[nameof(WeekText)];
        WeekHeaderText = Localizer[nameof(WeekHeaderText)];
        WeekNumberText = Localizer[nameof(WeekNumberText), GetWeekCount()];
        Months = Localizer[nameof(Months)].Value.Split(',').ToList();
    }

    protected DateTime StartDate
    {
        get
        {
            var d = Value.AddDays(1 - Value.Day);
            d = d.AddDays(0 - (int)d.DayOfWeek);
            return d;
        }
    }

    protected int GetWeekCount()
    {
        var gc = new GregorianCalendar();
        return gc.GetWeekOfYear(Value, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }

    protected DateTime EndDate => StartDate.AddDays(42);

    [Parameter]
    public DateTime Value { get; set; }

    [Parameter]
    public EventCallback<DateTime> ValueChanged { get; set; }

    [Parameter]
    public CalendarViewMode ViewMode { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment<CalendarCellValue>? CellTemplate { get; set; }

    protected async Task OnCellClickCallback(DateTime value)
    {
        Value = value;
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            StateHasChanged();
        }
    }

    protected async Task OnChangeYear(int offset)
    {
        Value = Value.AddYears(offset);
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    protected async Task OnChangeMonth(int offset)
    {
        if (offset == 0)
        {
            Value = DateTime.Today;
        }
        else
        {
            Value = Value.AddMonths(offset);
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    protected async Task OnChangeWeek(int offset)
    {
        if (offset == 0)
        {
            Value = DateTime.Today;
        }
        else
        {
            Value = Value.AddDays(offset);
        }
        WeekNumberText = Localizer[nameof(WeekNumberText), GetWeekCount()];
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private CalendarCellValue CreateCellValue(DateTime cellValue)
    {
        var val = new CalendarCellValue()
        {
            CellValue = cellValue,
            CalendarValue = Value
        };
        val.DefaultCss = GetCalendarCellString(val);
        return val;
    }
}
