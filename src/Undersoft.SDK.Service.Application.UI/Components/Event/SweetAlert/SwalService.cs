namespace Undersoft.SDK.Service.Application.Components;

public class SwalService : ComponentService<SwalOption>
{
    private ApplicationOptions _option;

    public SwalService(IOptionsMonitor<ApplicationOptions> option)
    {
        _option = option.CurrentValue;
    }

    public async Task Show(SwalOption option, SweetAlert? swal = null)
    {
        if (!option.ForceDelay && _option.SwalDelay != 0)
        {
            option.Delay = _option.SwalDelay;
        }

        await Invoke(option, swal);
    }
}
