using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("base/utility")]
public partial class Timer
{
    protected override string? ClassString => CssBuilder.Default("timer")
        .AddClass(base.ClassString)
        .Build();

    private string? PauseClassString => CssBuilder.Default("btn")
        .AddClass("btn-warning", !IsPause)
        .AddClass("btn-success", IsPause)
        .Build();

    private string? ValueString => $"{Math.Round(((1 - CurrentTimespan.TotalSeconds * 1.0 / Value.TotalSeconds) * CircleLength), 2)}";

    private TimeSpan CurrentTimespan { get; set; }

    private bool IsPause { get; set; }

    private string ValueTitleString => CurrentTimespan.Hours == 0 ? $"{CurrentTimespan:mm\\:ss}" : $"{CurrentTimespan:hh\\:mm\\:ss}";

    private string? AlertTime { get; set; }

    private CancellationTokenSource CancelTokenSource { get; set; } = new();

    private AutoResetEvent ResetEvent { get; } = new(false);

    private bool Vibrate { get; set; }

    [Parameter]
    public TimeSpan Value { get; set; }

    [Parameter]
    public override int Width { get; set; } = 300;

    [Parameter]
    public Func<Task>? OnTimeout { get; set; }

    [Parameter]
    public Func<Task>? OnCancel { get; set; }

    [Parameter]
    public override int StrokeWidth { get; set; } = 6;

    [Parameter]
    public bool IsVibrate { get; set; } = true;

    [Parameter]
    [NotNull]
    public string? PauseText { get; set; }

    [Parameter]
    [NotNull]
    public string? ResumeText { get; set; }

    [Parameter]
    [NotNull]
    public string? CancelText { get; set; }

    [Parameter]
    [NotNull]
    public string? StarText { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Timer>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PauseText ??= Localizer[nameof(PauseText)];
        ResumeText ??= Localizer[nameof(ResumeText)];
        CancelText ??= Localizer[nameof(CancelText)];
        StarText ??= Localizer[nameof(StarText)];

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.TimerIcon);
    }

    protected override Task ModuleInitAsync() => Task.CompletedTask;

    protected override async Task ModuleExecuteAsync()
    {
        if (Vibrate)
        {
            Vibrate = false;
            if (Module != null)
            {
                await Module.InvokeVoidAsync("vibrate");
            }
        }
    }

    private void OnStart()
    {
        IsPause = false;
        CurrentTimespan = Value;
        AlertTime = DateTime.Now.Add(CurrentTimespan).ToString("HH:mm:ss");

        StateHasChanged();

        Task.Run(async () =>
        {
            if (CancelTokenSource.IsCancellationRequested)
            {
                CancelTokenSource.Dispose();
                CancelTokenSource = new CancellationTokenSource();
            }

            while (!CancelTokenSource.IsCancellationRequested && CurrentTimespan > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(1000, CancelTokenSource.Token);
                }
                catch (TaskCanceledException) { }

                if (!CancelTokenSource.IsCancellationRequested)
                {
                    CurrentTimespan = CurrentTimespan.Subtract(TimeSpan.FromSeconds(1));
                    await InvokeAsync(StateHasChanged);
                }

                if (IsPause)
                {
                    ResetEvent.WaitOne();
                    AlertTime = DateTime.Now.Add(CurrentTimespan).ToString("HH:mm:ss");

                    CancelTokenSource.Dispose();
                    CancelTokenSource = new CancellationTokenSource();
                }
            }

            if (CurrentTimespan == TimeSpan.Zero)
            {
                await Task.Delay(500, CancelTokenSource.Token);
                if (!CancelTokenSource.IsCancellationRequested)
                {
                    Value = TimeSpan.Zero;
                    await InvokeAsync(async () =>
                    {
                        Vibrate = IsVibrate;
                        StateHasChanged();
                        if (OnTimeout != null)
                        {
                            await OnTimeout();
                        }
                    });
                }
            }
        });
    }

    private void OnClickPause()
    {
        IsPause = !IsPause;
        if (!IsPause)
        {
            ResetEvent.Set();
        }
        else
        {
            CancelTokenSource.Cancel();
        }
    }

    private string GetPauseText() => IsPause ? ResumeText : PauseText;

    private async Task OnClickCancel()
    {
        Value = TimeSpan.Zero;
        CancelTokenSource.Cancel();
        if (OnCancel != null)
        {
            await OnCancel();
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            CancelTokenSource.Cancel();
            CancelTokenSource.Dispose();

            ResetEvent.Dispose();
            if (Module != null)
            {
                await Module.DisposeAsync();
                Module = null;
            }
        }
    }
}
