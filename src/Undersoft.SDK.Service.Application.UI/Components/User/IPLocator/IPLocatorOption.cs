using Microsoft.Extensions.Logging;

namespace Undersoft.SDK.Service.Application.Components;

public class IPLocatorOption
{
    public Func<IServiceProvider, IIPLocator>? LocatorFactory { get; set; }

    public int RequestTimeout { get; set; } = 3000;

    public List<string> Localhosts { get; } = new List<string>(new string[] { "::1", "127.0.0.1" });

    protected internal string? IP { get; set; }

    protected internal HttpClient? HttpClient { get; set; }

    protected internal ILogger<IIPLocatorProvider>? Logger { get; set; }
}
