namespace Undersoft.SDK.Service.Application.Components;

public partial class SpeechWave : IDisposable
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public bool ShowUsedTime { get; set; } = true;

    [Parameter]
    public Func<Task>? OnTimeout { get; set; }

    [Parameter]
    public int TotalTime { get; set; } = 60000;

    private TimeSpan UsedTimeSpan { get; set; }

    private CancellationTokenSource? Token { get; set; }

    private string? ClassString => CssBuilder.Default("speech-wave")
        .AddClass("invisible", !Show)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? LineClassString => CssBuilder.Default("speech-wave-line")
        .AddClass("line", Show)
        .Build();

    private string? TotalTimeSpanString => $"{TimeSpan.FromMilliseconds(TotalTime):mm\\:ss}";

    private string? UsedTimeSpanString => $"{UsedTimeSpan:mm\\:ss}";

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Show)
        {
            Run();
        }
        else
        {
            Cancel();
        }
    }

    private bool IsRun { get; set; }

    private Task Run() => Task.Run(async () =>
    {
        if (!IsRun)
        {
            IsRun = true;
            UsedTimeSpan = TimeSpan.Zero;
            Token ??= new CancellationTokenSource();
            while (!Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, Token.Token);
                    UsedTimeSpan = UsedTimeSpan.Add(TimeSpan.FromSeconds(1));
                    if (UsedTimeSpan.TotalMilliseconds >= TotalTime)
                    {
                        Show = false;
                        if (OnTimeout != null)
                        {
                            await OnTimeout();
                        }
                    }
                    await InvokeAsync(StateHasChanged);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
            IsRun = false;
        }
    });

    private void Cancel()
    {
        if (Token != null)
        {
            Token.Cancel();
            Token.Dispose();
            Token = null;
        }
    }

    protected void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cancel();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
