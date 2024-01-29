namespace Undersoft.SDK.Service.Application.Components;

public class FullScreenService : ComponentService<FullScreenOption>
{
    public Task Toggle(FullScreenOption? option = null) => Invoke(option ?? new());
}
