using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Configuration;

namespace Undersoft.SDK.Service.Schedule.Quartz;

public class QuartzService
{
    private IScheduler _scheduler;

    public void ConfigureServices(ServiceConfigurationContext context)
    {
        var options = new QuartzOptions();

        context.Services.Manager.Configuration.Bind(options);

        context.Services.AddQuartz(options.Properties, build =>
        {
            // these are the defaults

            if (options.Properties[StdSchedulerFactory.PropertySchedulerTypeLoadHelperType] == null)
            {
                build.UseSimpleTypeLoader();
            }

            if (options.Properties[StdSchedulerFactory.PropertyJobStoreType] == null)
            {
                build.UseInMemoryStore();
            }

            if (options.Properties[StdSchedulerFactory.PropertyThreadPoolType] == null)
            {
                build.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });
            }

            if (options.Properties["quartz.plugin.timeZoneConverter.type"] == null)
            {
                build.UseTimeZoneConverter();
            }

            options.Configurator?.Invoke(build);
        });

        context.Services.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetRequiredService<ISchedulerFactory>().GetScheduler();
        });

        options.Properties = options.Properties;
        options.StartDelay = options.StartDelay;
    }

    public async Task OnApplicationInitializationAsync(IServiceRegistry context)
    {
        var options = context.GetRequiredService<IOptions<QuartzOptions>>().Value;

        _scheduler = context.GetRequiredService<IScheduler>();

        await options.StartSchedulerFactory.Invoke(_scheduler);
    }

    public async Task OnApplicationShutdownAsync(IServiceRegistry context)
    {
        if (_scheduler.IsStarted)
        {
            await _scheduler.Shutdown();
        }
    }

    public void OnApplicationInitialization(IServiceRegistry context)
    {
        OnApplicationShutdownAsync(context).RunSynchronously();
    }

    public void OnApplicationShutdown(IServiceRegistry context)
    {
        OnApplicationShutdownAsync(context).RunSynchronously();
    }
}
