namespace Undersoft.SDK.Service.Application.Components;

public class ClipboardService : ComponentService<ClipboardOption>
{
    public Task Copy(string? text, Func<Task>? callback = null) => Invoke(new ClipboardOption() { Text = text, Callback = callback });
}
