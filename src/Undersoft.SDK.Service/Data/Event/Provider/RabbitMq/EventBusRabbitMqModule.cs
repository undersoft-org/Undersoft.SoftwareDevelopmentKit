using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Undersoft.SDK.Service.Data.Event.Provider.RabbitMq;

using Configuration;

public class EventBusRabbitMqModule
{
    public void ConfigureServices(IServiceConfiguration configuration)
    {
        configuration.Configure<RabbitMqEventBusOptions>("RabbitMQ:EventBus");
    }

    public void OnApplicationInitialization()
    {
        ServiceManager.GetManager()
            .GetRequiredService<RabbitMqEventBus>()
            .Initialize();
    }
}