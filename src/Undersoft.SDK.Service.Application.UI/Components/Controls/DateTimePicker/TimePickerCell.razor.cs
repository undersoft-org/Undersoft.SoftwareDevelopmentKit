namespace Undersoft.SDK.Service.Application.Components;

public partial class TimePickerCell
{
    private string? ClassString => CssBuilder.Default("time-spinner-wrapper is-arrow")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? GetClassName(int index) => CssBuilder.Default("time-spinner-item")
        .AddClass("prev", ViewMode switch
        {
            TimePickerCellViewMode.Hour => Value.Hours - 1 == index,
            TimePickerCellViewMode.Minute => Value.Minutes - 1 == index,
            _ => Value.Seconds - 1 == index
        })
        .AddClass("active", ViewMode switch
        {
            TimePickerCellViewMode.Hour => Value.Hours == index,
            TimePickerCellViewMode.Minute => Value.Minutes == index,
            _ => Value.Seconds == index
        })
        .AddClass("next", ViewMode switch
        {
            TimePickerCellViewMode.Hour => Value.Hours + 1 == index,
            TimePickerCellViewMode.Minute => Value.Minutes + 1 == index,
            _ => Value.Seconds + 1 == index
        })
        .Build();

    private IEnumerable<int> Range => ViewMode switch
    {
        TimePickerCellViewMode.Hour => Enumerable.Range(0, 24),
        _ => Enumerable.Range(0, 60)
    };

    private string? StyleName => CssBuilder.Default()
        .AddClass($"transform: translateY({CalcTranslateY()}px);")
        .Build();

    private string? UpIconString => CssBuilder.Default("time-spinner-arrow time-up")
        .AddClass(UpIcon, !string.IsNullOrEmpty(UpIcon))
        .Build();

    private string? DownIconString => CssBuilder.Default("time-spinner-arrow time-down")
        .AddClass(DownIcon, !string.IsNullOrEmpty(DownIcon))
        .Build();

    [Parameter]
    public TimePickerCellViewMode ViewMode { get; set; }

    [Parameter]
    public TimeSpan Value { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ValueChanged { get; set; }

    [Parameter]
    public string? UpIcon { get; set; }

    [Parameter]
    public string? DownIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        UpIcon ??= IconTheme.GetIconByKey(ComponentIcons.TimePickerCellUpIcon);
        DownIcon ??= IconTheme.GetIconByKey(ComponentIcons.TimePickerCellDownIcon);
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop);

    [JSInvokable]
    public async Task OnClickUp()
    {
        var ts = ViewMode switch
        {
            TimePickerCellViewMode.Hour => TimeSpan.FromHours(1),
            TimePickerCellViewMode.Minute => TimeSpan.FromMinutes(1),
            _ => TimeSpan.FromSeconds(1),
        };
        Value = Value.Subtract(ts);
        if (Value < TimeSpan.Zero)
        {
            Value = Value.Add(TimeSpan.FromHours(24));
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    [JSInvokable]
    public void OnHeightCallback(double height) => _height = height;

    [JSInvokable]
    public async Task OnClickDown()
    {
        var ts = ViewMode switch
        {
            TimePickerCellViewMode.Hour => TimeSpan.FromHours(1),
            TimePickerCellViewMode.Minute => TimeSpan.FromMinutes(1),
            _ => TimeSpan.FromSeconds(1)
        };
        Value = Value.Add(ts);
        if (Value.Days > 0)
        {
            Value = Value.Subtract(TimeSpan.FromDays(1));
        }

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private double? _height;

    private double CalcTranslateY()
    {
        var height = _height ?? 36.59375d;
        return 0 - ViewMode switch
        {
            TimePickerCellViewMode.Hour => (Value.Hours) * height,
            TimePickerCellViewMode.Minute => (Value.Minutes) * height,
            _ => (Value.Seconds) * height
        };
    }
}
