using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("dropdown", ModuleName = "Dropdown")]
public partial class DateTimePicker<TValue>
{
    private string? ClassString => CssBuilder.Default("select datetime-picker")
        .AddClass("disabled", IsDisabled)
        .AddClass(ValidCss)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? InputClassName => CssBuilder.Default("dropdown-toggle form-control datetime-picker-Input")
        .AddClass(ValidCss)
        .Build();

    private string? DateTimePickerIconClassString => CssBuilder.Default("datetime-picker-bar")
        .AddClass(Icon)
        .Build();

    private string? TabIndexString => ValidateForm != null ? "0" : null;

    private string? AutoCloseString => AutoClose ? "true" : null;

    private string? PlaceholderString => ViewMode switch
    {
        DatePickerViewMode.DateTime => DateTimePlaceHolderText,
        _ => DatePlaceHolderText
    };

    private bool AllowNull { get; set; }

    [Parameter]
    public string? Format { get; set; }

    [Parameter]
    [NotNull]
    public string? Icon { get; set; }

    [Parameter]
    public DatePickerViewMode ViewMode { get; set; } = DatePickerViewMode.Date;

    [Parameter]
    public bool ShowSidebar { get; set; }

    [Parameter]
    [NotNull]
    public RenderFragment<Func<DateTime, Task>>? SidebarTemplate { get; set; }

    [Parameter]
    public DateTime? MaxValue { get; set; }

    [Parameter]
    public DateTime? MinValue { get; set; }

    [Parameter]
    public bool AutoClose { get; set; } = true;

    [Parameter]
    public bool AutoToday { get; set; } = true;

    [Inject]
    [NotNull]
    private IStringLocalizer<DateTimePicker<DateTime>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    [NotNull]
    private string? DatePlaceHolderText { get; set; }

    [NotNull]
    private string? DateTimePlaceHolderText { get; set; }

    [NotNull]
    private string? GenericTypeErroMessage { get; set; }

    [NotNull]
    private string? DateTimeFormat { get; set; }

    [NotNull]
    private string? DateFormat { get; set; }

    private DateTime SelectedValue { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DateTimePlaceHolderText ??= Localizer[nameof(DateTimePlaceHolderText)];
        DatePlaceHolderText ??= Localizer[nameof(DatePlaceHolderText)];
        GenericTypeErroMessage ??= Localizer[nameof(GenericTypeErroMessage)];
        DateTimeFormat ??= Localizer[nameof(DateTimeFormat)];
        DateFormat ??= Localizer[nameof(DateFormat)];

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.DateTimePickerIcon);

        var type = typeof(TValue);

        if (!type.IsDateTime())
        {
            throw new InvalidOperationException(GenericTypeErroMessage);
        }

        AllowNull = Nullable.GetUnderlyingType(type) != null;

        if (!string.IsNullOrEmpty(Format))
        {
            DateTimeFormat = Format;

            var index = Format.IndexOf(' ');
            if (index > 0)
            {
                DateFormat = Format[..index];
            }
        }

        if (AutoToday && (Value == null || Value.ToString() == DateTime.MinValue.ToString()))
        {
            SelectedValue = DateTime.Today;
            if (!AllowNull)
            {
                CurrentValueAsString = SelectedValue.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        else if (Value is DateTime dt)
        {
            SelectedValue = dt;
        }
        else
        {
            var offset = (DateTimeOffset?)(object)Value;
            SelectedValue = offset.HasValue
                ? offset.Value.DateTime
                : DateTime.MinValue;
        }
    }

    protected override string FormatValueAsString(TValue value)
    {
        var ret = "";
        if (value != null)
        {
            var format = Format;
            if (string.IsNullOrEmpty(format))
            {
                format = ViewMode == DatePickerViewMode.DateTime ? DateTimeFormat : DateFormat;
            }

            ret = SelectedValue.ToString(format);
        }
        return ret;
    }

    private Task OnClear()
    {
        SelectedValue = AutoToday ? DateTime.Today : DateTime.MinValue;
        CurrentValue = default;
        return Task.CompletedTask;
    }

    private async Task OnConfirm()
    {
        CurrentValueAsString = SelectedValue.ToString("yyyy-MM-dd HH:mm:ss");
        if (AutoClose)
        {
            await InvokeExecuteAsync(Id, "hide");
        }
    }
}
