using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Undersoft.SDK.Service.Configuration;

namespace Undersoft.SDK.Service.Hosting
{
    public class ServiceHostBuilder
    { 
        IServicer _servicer;

        public ServiceHostBuilder() { }

        public ServiceHostBuilder(IServicer servicer)
        {
            _servicer = servicer;
        }

        public IHost Build<TStartup>(Type[] serviceClients, string[] args) where TStartup : class, IHostedService
        {
            var config = ServiceConfigurationHelper.BuildConfiguration();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory<IServiceCollection>(_servicer.GetProviderFactory())
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                    configHost.AddConfiguration(config);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddServiceSetup(config)
                            .ConfigureServices(serviceClients);
                    services.AddHostedService<TStartup>();
                })
                .Build();

            return host;
        }

        public async Task Run<TStartup>(Type[] serviceClients, string[] args) where TStartup : class, IHostedService
        {
            var host = Build<TStartup>(serviceClients, args);

            using (host)
            {
                await host.StartAsync();

                await host.WaitForShutdownAsync();
            }
        }
    }
}