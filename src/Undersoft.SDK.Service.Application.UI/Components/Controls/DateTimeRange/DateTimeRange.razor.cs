using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("dropdown", ModuleName = "Dropdown")]
public partial class DateTimeRange
{
    private string? ClassString => CssBuilder.Default("select datetime-range form-control")
        .AddClass("disabled", IsDisabled)
        .AddClass(ValidCss)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? DateTimePickerIconClassString => CssBuilder.Default("range-bar")
        .AddClass(Icon)
        .AddClass("disabled", IsDisabled)
        .Build();

    internal DateTimeRangeValue SelectedValue { get; } = new DateTimeRangeValue();

    private DateTime StartValue { get; set; }

    private string? StartValueString => (Value == null || Value.Start == DateTime.MinValue) ? null : Value.Start.ToString(DateFormat);

    private DateTime EndValue { get; set; }

    private string? EndValueString => (Value == null || Value.End == DateTime.MinValue) ? null : Value.End.ToString(DateFormat);

    [NotNull]
    private string? StartPlaceHolderText { get; set; }

    [NotNull]
    private string? EndPlaceHolderText { get; set; }

    [NotNull]
    private string? SeparateText { get; set; }

    [NotNull]
    private string? DateFormat { get; set; }

    [Parameter]
    public bool AutoCloseClickSideBar { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearButtonText { get; set; }

    [Parameter]
    public string? ClearIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? TodayButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmButtonText { get; set; }

    [Parameter]
    public DateTime MaxValue { get; set; } = DateTime.MaxValue;
    [Parameter]
    public DateTime MinValue { get; set; } = DateTime.MinValue;

    [Parameter]
    public bool AllowNull { get; set; } = true;

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public bool ShowToday { get; set; }

    [Parameter]
    public bool ShowSidebar { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<DateTimeRangeSidebarItem>? SidebarItems { get; set; }

    [Parameter]
    public Func<DateTimeRangeValue, Task>? OnConfirm { get; set; }

    [Parameter]
    public Func<DateTimeRangeValue, Task>? OnClearValue { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<DateTimeRange>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizerFactory? LocalizerFactory { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Value ??= new DateTimeRangeValue();

        StartValue = Value.Start;
        EndValue = Value.End;

        if (StartValue == DateTime.MinValue) StartValue = DateTime.Today;
        if (EndValue == DateTime.MinValue) EndValue = StartValue.AddMonths(1);

        SelectedValue.Start = StartValue;
        SelectedValue.End = EndValue;

        StartPlaceHolderText ??= Localizer[nameof(StartPlaceHolderText)];
        EndPlaceHolderText ??= Localizer[nameof(EndPlaceHolderText)];
        SeparateText ??= Localizer[nameof(SeparateText)];

        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        ConfirmButtonText ??= Localizer[nameof(ConfirmButtonText)];
        TodayButtonText ??= Localizer[nameof(TodayButtonText)];

        DateFormat ??= Localizer[nameof(DateFormat)];

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.DateTimeRangeIcon);
        ClearIcon ??= IconTheme.GetIconByKey(ComponentIcons.DateTimeRangeClearIcon); ;

        if (StartValue.ToString("yyyy-MM") == EndValue.ToString("yyyy-MM"))
        {
            StartValue = StartValue.AddMonths(-1);
        }

        SidebarItems ??= new DateTimeRangeSidebarItem[]
        {
            new DateTimeRangeSidebarItem{ Text = Localizer["Last7Days"], StartDateTime = DateTime.Today.AddDays(-7), EndDateTime = DateTime.Today },
            new DateTimeRangeSidebarItem{ Text = Localizer["Last30Days"], StartDateTime = DateTime.Today.AddDays(-30), EndDateTime = DateTime.Today },
            new DateTimeRangeSidebarItem{ Text = Localizer["ThisMonth"], StartDateTime = DateTime.Today.AddDays(1- DateTime.Today.Day), EndDateTime = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1).AddDays(-1) },
            new DateTimeRangeSidebarItem{ Text = Localizer["LastMonth"], StartDateTime = DateTime.Today.AddDays(1- DateTime.Today.Day).AddMonths(-1), EndDateTime = DateTime.Today.AddDays(1- DateTime.Today.Day).AddDays(-1) },
        };
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (EditContext != null && FieldIdentifier != null)
        {
            var pi = FieldIdentifier.Value.Model.GetType().GetPropertyByName(FieldIdentifier.Value.FieldName);
            if (pi != null)
            {
                var required = pi.GetCustomAttribute<RequiredAttribute>(true);
                if (required != null)
                {
                    Rules.Add(new DateTimeRangeRequiredValidator()
                    {
                        LocalizerFactory = LocalizerFactory,
                        ErrorMessage = required.ErrorMessage,
                        AllowEmptyString = required.AllowEmptyStrings
                    });
                }
            }
        }
    }

    private async Task OnClickSidebarItem(DateTimeRangeSidebarItem item)
    {
        SelectedValue.Start = item.StartDateTime;
        SelectedValue.End = item.EndDateTime;
        StartValue = SelectedValue.Start;
        EndValue = SelectedValue.End;

        if (AutoCloseClickSideBar)
        {
            await InvokeExecuteAsync(Id, "hide");
            await ClickConfirmButton();
        }
    }

    private async Task ClickClearButton()
    {
        Value = new DateTimeRangeValue();

        if (OnClearValue != null)
        {
            await OnClearValue(Value);
        }
        if (OnValueChanged != null)
        {
            await OnValueChanged(Value);
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        if (IsNeedValidate && EditContext != null && FieldIdentifier != null)
        {
            EditContext.NotifyFieldChanged(FieldIdentifier.Value);
        }
    }

    private async Task ClickTodayButton()
    {
        SelectedValue.Start = DateTime.Today;
        SelectedValue.End = DateTime.Today;
        StartValue = DateTime.Today;
        EndValue = StartValue.AddMonths(1);
        await ClickConfirmButton();
    }

    private async Task ClickConfirmButton()
    {
        if (SelectedValue.End == DateTime.MinValue)
        {
            if (SelectedValue.Start < DateTime.Today)
            {
                SelectedValue.End = DateTime.Today;
            }
            else
            {
                SelectedValue.End = SelectedValue.Start;
                SelectedValue.Start = DateTime.Today;
            }
        }
        Value.Start = SelectedValue.Start;
        Value.End = SelectedValue.End.Date.AddDays(1).AddSeconds(-1);

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        if (OnConfirm != null)
        {
            await OnConfirm(Value);
        }
        if (OnValueChanged != null)
        {
            await OnValueChanged(Value);
        }
        if (IsNeedValidate && EditContext != null && FieldIdentifier != null)
        {
            EditContext.NotifyFieldChanged(FieldIdentifier.Value);
        }
    }

    internal void UpdateStart(DateTime d)
    {
        StartValue = d;
        EndValue = StartValue.AddMonths(1);
        StateHasChanged();
    }

    internal void UpdateEnd(DateTime d)
    {
        EndValue = d;
        StartValue = EndValue.AddMonths(-1);
        StateHasChanged();
    }

    internal void UpdateValue(DateTime d)
    {
        if (SelectedValue.End == DateTime.MinValue)
        {
            if (d < SelectedValue.Start)
            {
                SelectedValue.End = SelectedValue.Start;
                SelectedValue.Start = d;
            }
            else
            {
                SelectedValue.End = d;
            }
        }
        else
        {
            SelectedValue.Start = d;
            SelectedValue.End = DateTime.MinValue;
        }

        var startDate = StartValue.AddDays(1 - StartValue.Day);
        if (d < startDate)
        {
            UpdateStart(d);
        }
        else if (d > startDate.AddMonths(2).AddDays(-1))
        {
            UpdateEnd(d);
        }
        else
        {
            StateHasChanged();
        }
    }

    public override bool IsComplexValue(object? propertyValue) => false;
}
