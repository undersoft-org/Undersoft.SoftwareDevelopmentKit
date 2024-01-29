namespace Undersoft.SDK.Service.Application.Components;

public interface IIPLocator
{
    Task<string?> Locate(IPLocatorOption option);

    public string? Url { get; set; }
}
