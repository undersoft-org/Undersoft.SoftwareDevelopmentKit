using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Undersoft.SDK.Service.Application.Components;

public class DefaultIPLocator : IIPLocator
{
    public virtual Task<string?> Locate(IPLocatorOption option) => Task.FromResult<string?>(null);

    public string? Url { get; set; }

    protected virtual async Task<string?> Locate<T>(IPLocatorOption option) where T : class
    {
        string? ret = null;
        if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(option.IP) && option.HttpClient != null)
        {
            var url = string.Format(Url, option.IP);
            try
            {
                using var token = new CancellationTokenSource(option.RequestTimeout);
                var result = await option.HttpClient.GetFromJsonAsync<T>(url, token.Token);
                if (result != null)
                {
                    ret = result.ToString();
                }
            }
            catch (Exception ex)
            {
                option.Logger?.LogError(ex, "Url: {url}", url);
            }
        }
        return ret;
    }
}
