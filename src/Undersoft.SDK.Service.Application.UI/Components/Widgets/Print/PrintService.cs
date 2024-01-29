namespace Undersoft.SDK.Service.Application.Components;

public class PrintService : ComponentService<DialogOption>
{
    public Task PrintAsync(DialogOption option) => Invoke(option);
}
