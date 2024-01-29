namespace Undersoft.SDK.Service.Application.Components;

public class ToastService : ComponentService<ToastOption>
{
    private ApplicationOptions Options { get; }

    public ToastService(IOptionsMonitor<ApplicationOptions> options)
    {
        Options = options.CurrentValue;
    }

    public async Task Show(ToastOption option, ToastContainer? ToastContainer = null)
    {
        if (!option.ForceDelay && Options.ToastDelay != 0)
        {
            option.Delay = Options.ToastDelay;
        }
        await Invoke(option, ToastContainer);
    }
}
