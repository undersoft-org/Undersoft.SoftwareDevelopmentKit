using Microsoft.Extensions.Hosting;

namespace Undersoft.SDK.Service.Hosting
{
    public interface IServiceHost : IHost, IHostedService
    {
        IHost Host { get; set; }
        string HostName { get; set; }
        int Port { get; set; }
        string Route { get; set; }
        string Name { get; set; }
        long TenantId { get; set; }
        string TenantName { get; set; }
        string TypeName { get; set; }
    }
}