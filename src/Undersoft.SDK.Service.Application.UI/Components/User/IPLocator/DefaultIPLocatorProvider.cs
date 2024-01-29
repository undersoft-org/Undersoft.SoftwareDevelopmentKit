using Microsoft.Extensions.Logging;

namespace Undersoft.SDK.Service.Application.Components;

internal class DefaultIPLocatorProvider : IIPLocatorProvider
{
    private readonly IPLocatorOption _option;

    private readonly IServiceProvider _provider;

    public DefaultIPLocatorProvider(IServiceProvider provider, IHttpClientFactory factory, ILogger<DefaultIPLocatorProvider> logger, IOptionsMonitor<IPLocatorOption> option)
    {
        _provider = provider;
        _option = option.CurrentValue;
        _option.HttpClient = factory.CreateClient();
        _option.Logger = logger;
    }

    public async Task<string?> Locate(string ip)
    {
        string? ret = null;

        if (string.IsNullOrEmpty(ip) || _option.Localhosts.Any(p => p == ip))
        {
            ret = "本地连接";
        }
        else
        {
            _option.IP = ip;
            if (_option.LocatorFactory != null)
            {
                var locator = _option.LocatorFactory(_provider);
                if (locator != null)
                {
                    ret = await locator.Locate(_option);
                }
            }
        }
        return ret;
    }
}
