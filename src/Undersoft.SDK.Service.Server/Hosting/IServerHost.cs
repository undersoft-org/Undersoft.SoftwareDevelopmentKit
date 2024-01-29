using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Undersoft.SDK.Service.Hosting;

namespace Undersoft.SDK.Service.Server.Hosting
{
    public interface IServerHost : IServiceHost
    {
        ServerHost Configure(Action<IWebHostBuilder> builder);

        void Run();

        Registry<IServiceProvider> HostedServices { get; }

        Registry<ServiceHost> ServiceHosts { get; set; }
    }
}