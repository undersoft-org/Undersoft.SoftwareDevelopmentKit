namespace Undersoft.SDK.Service.Application.Components;

public class MessageService : ComponentService<MessageOption>
{
    private ApplicationOptions Options { get; }

    public MessageService(IOptionsMonitor<ApplicationOptions> option)
    {
        Options = option.CurrentValue;
    }

    public async Task Show(MessageOption option, Message? message = null)
    {
        if (!option.ForceDelay)
        {
            if (Options.MessageDelay != 0)
            {
                option.Delay = Options.MessageDelay;
            }
        }
        await Invoke(option, message);
    }
}
