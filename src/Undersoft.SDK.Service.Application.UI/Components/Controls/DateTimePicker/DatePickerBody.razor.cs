using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class DatePickerBody
{
    private DateTime StartDate
    {
        get
        {
            var d = GetSafeDayDateTime(CurrentDate, 1 - CurrentDate.Day);
            d = GetSafeDayDateTime(d, 0 - (int)d.DayOfWeek);
            return d;
        }
    }

    private DateTime EndDate => GetSafeDayDateTime(StartDate, 42);

    private DateTime CurrentDate { get; set; }

    private TimeSpan CurrentTime { get; set; }

    private bool ShowTimePicker { get; set; }

    private string? ClassString => CssBuilder.Default("picker-panel")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? GetDayClass(DateTime day, bool overflow) => CssBuilder.Default("")
        .AddClass("prev-month", day.Month < CurrentDate.Month)
        .AddClass("next-month", day.Month > CurrentDate.Month)
        .AddClass("current", day == Value && Ranger == null && day.Month == CurrentDate.Month && !overflow)
        .AddClass("start", Ranger != null && day == Ranger.SelectedValue.Start.Date)
        .AddClass("end", Ranger != null && day == Ranger.SelectedValue.End.Date)
        .AddClass("range", Ranger != null && day >= Ranger.SelectedValue.Start && day <= Ranger.SelectedValue.End)
        .AddClass("today", day == DateTime.Today)
        .AddClass("disabled", IsDisabled(day) || overflow)
        .Build();

    private bool IsDisabled(DateTime day) => (MinValue.HasValue && day < MinValue.Value) || (MaxValue.HasValue && day > MaxValue.Value);

    private string? DateTimeViewClassName => CssBuilder.Default("date-picker-time-header")
        .AddClass("d-none", ViewMode != DatePickerViewMode.DateTime)
        .AddClass("is-open", ShowTimePicker)
        .Build();

    private string? PrevMonthClassName => CssBuilder.Default("picker-panel-icon-btn pick-panel-arrow-left")
        .AddClass("d-none", CurrentViewMode == DatePickerViewMode.Year || CurrentViewMode == DatePickerViewMode.Month)
        .Build();

    private string? NextMonthClassName => CssBuilder.Default("picker-panel-icon-btn pick-panel-arrow-right")
        .AddClass("d-none", CurrentViewMode == DatePickerViewMode.Year || CurrentViewMode == DatePickerViewMode.Month)
        .Build();

    private string? DateViewClassName => CssBuilder.Default("date-table")
        .AddClass("d-none", CurrentViewMode == DatePickerViewMode.Year || CurrentViewMode == DatePickerViewMode.Month)
        .Build();

    private string? YearViewClassName => CssBuilder.Default("year-table")
        .AddClass("d-none", CurrentViewMode != DatePickerViewMode.Year)
        .Build();

    private string? MonthViewClassName => CssBuilder.Default("month-table")
        .AddClass("d-none", CurrentViewMode != DatePickerViewMode.Month)
        .Build();

    private string? CurrentMonthViewClassName => CssBuilder.Default("date-picker-header-label")
        .AddClass("d-none", CurrentViewMode == DatePickerViewMode.Year || CurrentViewMode == DatePickerViewMode.Month)
        .Build();

    [NotNull]
    private string? YearText { get; set; }

    private string? YearString => CurrentViewMode switch
    {
        DatePickerViewMode.Year => GetYearPeriod(),
        _ => string.Format(YearText, CurrentDate.Year)
    };

    [NotNull]
    private string? MonthText { get; set; }

    private string MonthString => string.Format(MonthText, Months.ElementAtOrDefault(CurrentDate.Month - 1));

    [NotNull]
    private string? YearPeriodText { get; set; }

    private string? DateValueString => CurrentDate.ToString(DateFormat);

    private string? TimeValueString => CurrentTime.ToString(TimeFormat);

    private DatePickerViewMode CurrentViewMode { get; set; }

    [Parameter]
    public DatePickerViewMode ViewMode { get; set; } = DatePickerViewMode.Date;

    [Parameter]
    [NotNull]
    public string? DateFormat { get; set; }

    [Parameter]
    public bool ShowSidebar { get; set; }

    [Parameter]
    public RenderFragment<Func<DateTime, Task>>? SidebarTemplate { get; set; }

    [Parameter]
    public bool ShowLeftButtons { get; set; } = true;

    [Parameter]
    public bool ShowRightButtons { get; set; } = true;

    [Parameter]
    public bool ShowFooter { get; set; }

    [Parameter]
    [NotNull]
    public string? TimeFormat { get; set; }

    [Parameter]
    [NotNull]
    public string? TimePlaceHolder { get; set; }

    [Parameter]
    [NotNull]
    public string? DatePlaceHolder { get; set; }

    [Parameter]
    public bool AllowNull { get; set; }

    [Parameter]
    public bool AutoClose { get; set; }

    [Parameter]
    public Func<Task>? OnConfirm { get; set; }

    [Parameter]
    public Func<Task>? OnClear { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? NowButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    public DateTime Value { get; set; }

    [Parameter]
    public EventCallback<DateTime> ValueChanged { get; set; }

    [Parameter]
    public DateTime? MaxValue { get; set; }

    [Parameter]
    public DateTime? MinValue { get; set; }

    [Parameter]
    public string? PreviousYearIcon { get; set; }

    [Parameter]
    public string? NextYearIcon { get; set; }

    [Parameter]
    public string? PreviousMonthIcon { get; set; }

    [Parameter]
    public string? NextMonthIcon { get; set; }

    [CascadingParameter]
    private DateTimeRange? Ranger { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<DateTimePicker<DateTime>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    [NotNull]
    private string? AiraPrevYearLabel { get; set; }

    [NotNull]
    private string? AiraNextYearLabel { get; set; }

    [NotNull]
    private string? AiraPrevMonthLabel { get; set; }

    [NotNull]
    private string? AiraNextMonthLabel { get; set; }

    [NotNull]
    private List<string>? Months { get; set; }

    [NotNull]
    private List<string>? MonthLists { get; set; }

    [NotNull]
    private List<string>? WeekLists { get; set; }

    [NotNull]
    private string? Today { get; set; }

    [NotNull]
    private string? Yesterday { get; set; }

    [NotNull]
    private string? Week { get; set; }

    private Dictionary<DatePickerViewMode, List<DatePickerViewMode>> AllowSwitchModes { get; } = new Dictionary<DatePickerViewMode, List<DatePickerViewMode>>
    {
        [DatePickerViewMode.DateTime] = new List<DatePickerViewMode>()
        {
            DatePickerViewMode.DateTime,
            DatePickerViewMode.Month,
            DatePickerViewMode.Year
        },
        [DatePickerViewMode.Date] = new List<DatePickerViewMode>()
        {
            DatePickerViewMode.Date,
            DatePickerViewMode.Month,
            DatePickerViewMode.Year
        },
        [DatePickerViewMode.Month] = new List<DatePickerViewMode>()
        {
            DatePickerViewMode.Month,
            DatePickerViewMode.Year
        },
        [DatePickerViewMode.Year] = new List<DatePickerViewMode>()
        {
            DatePickerViewMode.Year
        }
    };

    protected override void OnInitialized()
    {
        base.OnInitialized();

        CurrentViewMode = ViewMode;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        CurrentDate = Value.Date;
        CurrentTime = Value - CurrentDate;

        DatePlaceHolder ??= Localizer[nameof(DatePlaceHolder)];
        TimePlaceHolder ??= Localizer[nameof(TimePlaceHolder)];
        TimeFormat ??= Localizer[nameof(TimeFormat)];
        DateFormat ??= Localizer[nameof(DateFormat)];

        AiraPrevYearLabel ??= Localizer[nameof(AiraPrevYearLabel)];
        AiraNextYearLabel ??= Localizer[nameof(AiraNextYearLabel)];
        AiraPrevMonthLabel ??= Localizer[nameof(AiraPrevMonthLabel)];
        AiraNextMonthLabel ??= Localizer[nameof(AiraNextMonthLabel)];

        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        NowButtonText ??= Localizer[nameof(NowButtonText)];
        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];

        YearText ??= Localizer[nameof(YearText)];
        MonthText ??= Localizer[nameof(MonthText)];
        YearPeriodText ??= Localizer[nameof(YearPeriodText)];
        MonthLists = Localizer[nameof(MonthLists)].Value.Split(',').ToList();
        Months = Localizer[nameof(Months)].Value.Split(',').ToList();
        WeekLists = Localizer[nameof(WeekLists)].Value.Split(',').ToList();

        Today ??= Localizer[nameof(Today)];
        Yesterday ??= Localizer[nameof(Yesterday)];
        Week ??= Localizer[nameof(Week)];

        PreviousYearIcon ??= IconTheme.GetIconByKey(ComponentIcons.DatePickBodyPreviousYearIcon);
        PreviousMonthIcon ??= IconTheme.GetIconByKey(ComponentIcons.DatePickBodyPreviousMonthIcon);
        NextMonthIcon ??= IconTheme.GetIconByKey(ComponentIcons.DatePickBodyNextMonthIcon);
        NextYearIcon ??= IconTheme.GetIconByKey(ComponentIcons.DatePickBodyNextYearIcon);
    }

    private void SetValue(DateTime val)
    {
        if (val != Value)
        {
            Value = val;
            CurrentDate = Value.Date;
            CurrentTime = Value - CurrentDate;
        }
    }

    private void OnClickPrevYear()
    {
        ShowTimePicker = false;
        CurrentDate = CurrentViewMode == DatePickerViewMode.Year ? GetSafeYearDateTime(CurrentDate, -20) : GetSafeYearDateTime(CurrentDate, -1);
        Ranger?.UpdateStart(CurrentDate);
    }

    private void OnClickPrevMonth()
    {
        ShowTimePicker = false;
        CurrentDate = GetSafeMonthDateTime(CurrentDate, -1);
        Ranger?.UpdateStart(CurrentDate);
    }

    private void OnClickNextYear()
    {
        ShowTimePicker = false;
        CurrentDate = CurrentViewMode == DatePickerViewMode.Year ? GetSafeYearDateTime(CurrentDate, 20) : GetSafeYearDateTime(CurrentDate, 1);
        Ranger?.UpdateEnd(CurrentDate);
    }

    private void OnClickNextMonth()
    {
        ShowTimePicker = false;
        CurrentDate = GetSafeMonthDateTime(CurrentDate, 1);
        Ranger?.UpdateEnd(CurrentDate);
    }

    private async Task OnClickDateTime(DateTime d)
    {
        ShowTimePicker = false;
        SetValue(d + CurrentTime);
        Ranger?.UpdateValue(d);
        if (Ranger == null)
        {
            if (!ShowFooter || AutoClose)
            {
                await ClickConfirmButton();
            }
            else
            {
                StateHasChanged();
            }
        }
    }

    private async Task SwitchView(DatePickerViewMode view)
    {
        ShowTimePicker = false;
        SetValue(CurrentDate);
        if (AllowSwitchModes[ViewMode].Contains(view))
        {
            CurrentViewMode = view;
            StateHasChanged();
        }
        else if (AutoClose)
        {
            await ClickConfirmButton();
        }
    }

    private async Task SwitchView(DatePickerViewMode view, DateTime d)
    {
        CurrentDate = d;
        await SwitchView(view);
    }

    private string GetYearPeriod()
    {
        var start = GetSafeYearDateTime(CurrentDate, 0 - CurrentDate.Year % 20).Year;
        return string.Format(YearPeriodText, start, start + 19);
    }

    private DateTime GetYear(int year) => GetSafeYearDateTime(CurrentDate, year - (CurrentDate.Year % 20));

    private string GetYearText(int year) => GetYear(year).Year.ToString();

    private string? GetYearClassName(int year, bool overflow) => CssBuilder.Default()
        .AddClass("current", GetSafeYearDateTime(CurrentDate, year - (CurrentDate.Year % 20)).Year == Value.Year)
        .AddClass("today", GetSafeYearDateTime(CurrentDate, year - (CurrentDate.Year % 20)).Year == DateTime.Today.Year)
        .AddClass("disabled", overflow)
        .Build();

    private DateTime GetMonth(int month) => GetSafeMonthDateTime(CurrentDate, month - CurrentDate.Month);

    private string? GetMonthClassName(int month) => CssBuilder.Default()
        .AddClass("current", month == Value.Month)
        .AddClass("today", CurrentDate.Year == DateTime.Today.Year && month == DateTime.Today.Month)
        .Build();

    private static string GetDayText(int day) => day.ToString();

    private string GetMonthText(int month) => MonthLists[month - 1];

    private void OnClickTimeInput() => ShowTimePicker = true;

    private async Task ClickNowButton()
    {
        var val = ViewMode switch
        {
            DatePickerViewMode.DateTime => DateTime.Now,
            _ => DateTime.Today
        };
        SetValue(val);
        await ClickConfirmButton();
    }

    private async Task ClickClearButton()
    {
        ShowTimePicker = false;
        if (OnClear != null)
        {
            await OnClear();
        }
    }

    private async Task ClickConfirmButton()
    {
        ShowTimePicker = false;
        if (Validate() && ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        if (OnConfirm != null)
        {
            await OnConfirm();
        }
    }

    private bool Validate() => (!MinValue.HasValue || Value >= MinValue.Value) && (!MaxValue.HasValue || Value <= MaxValue.Value);

    private void OnTimePickerClose()
    {
        SetValue(CurrentDate + CurrentTime);
        ShowTimePicker = false;
        StateHasChanged();
    }

    protected static DateTime GetSafeYearDateTime(DateTime dt, int year)
    {
        var @base = dt;
        if (year < 0)
        {
            if (DateTime.MinValue.AddYears(0 - year) < dt)
            {
                @base = dt.AddYears(year);
            }
            else
            {
                @base = DateTime.MinValue.Date;
            }
        }
        else if (year > 0)
        {
            if (DateTime.MaxValue.AddYears(0 - year) > dt)
            {
                @base = dt.AddYears(year);
            }
            else
            {
                @base = DateTime.MaxValue.Date;
            }
        }
        return @base;
    }

    protected static DateTime GetSafeMonthDateTime(DateTime dt, int month)
    {
        var @base = dt;
        if (month < 0)
        {
            if (DateTime.MinValue.AddMonths(0 - month) < dt)
            {
                @base = dt.AddMonths(month);
            }
            else
            {
                @base = DateTime.MinValue.Date;
            }
        }
        else if (month > 0)
        {
            if (DateTime.MaxValue.AddMonths(0 - month) > dt)
            {
                @base = dt.AddMonths(month);
            }
            else
            {
                @base = DateTime.MaxValue.Date;
            }
        }
        return @base;
    }

    protected static DateTime GetSafeDayDateTime(DateTime dt, int day)
    {
        var @base = dt;
        if (day < 0)
        {
            if (DateTime.MinValue.AddDays(0 - day) < dt)
            {
                @base = dt.AddDays(day);
            }
            else
            {
                @base = DateTime.MinValue;
            }
        }
        else if (day > 0)
        {
            if (DateTime.MaxValue.AddDays(0 - day) > dt)
            {
                @base = dt.AddDays(day);
            }
            else
            {
                @base = DateTime.MaxValue;
            }
        }
        return @base;
    }

    protected static bool IsDayOverflow(DateTime dt, int day) => DateTime.MaxValue.AddDays(0 - day) < dt;

    protected static bool IsYearOverflow(DateTime dt, int year)
    {
        var ret = false;
        if (year < 0)
        {
            ret = DateTime.MinValue.AddYears(0 - year) > dt;
        }
        else if (year > 0)
        {
            ret = DateTime.MaxValue.AddYears(0 - year) < dt;
        }
        return ret;
    }
}
