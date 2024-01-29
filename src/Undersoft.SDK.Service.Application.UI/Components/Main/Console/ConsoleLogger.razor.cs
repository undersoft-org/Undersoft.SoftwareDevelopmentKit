using System.Collections.Concurrent;

namespace Undersoft.SDK.Service.Application.Components;

public partial class ConsoleLogger
{
    [Parameter]
    public int Max { get; set; } = 3;

    [Parameter]
    public bool IsHtml { get; set; }

    private ConcurrentQueue<string> Message { get; } = new();

    private string? ClassName => CssBuilder.Default("console-logger")
        .AddClass(Class)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string Class { get; set; } = "collapse";

    public void Log(string message)
    {
        Message.Enqueue($"{DateTimeOffset.Now}: {message}");
        Class = "";
        if (Message.Count > Max)
        {
            Message.TryDequeue(out _);
        }
        StateHasChanged();
    }
}
