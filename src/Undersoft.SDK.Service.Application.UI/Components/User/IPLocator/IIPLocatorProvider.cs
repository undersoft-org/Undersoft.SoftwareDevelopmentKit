namespace Undersoft.SDK.Service.Application.Components;

public interface IIPLocatorProvider
{
    Task<string?> Locate(string ip);
}
