using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetTopologySuite.Utilities;
using Quartz;

namespace Undersoft.SDK.Service.Schedule.Quartz;

public class QuartzOptions
{
    public NameValueCollection Properties { get; set; }

    public Action<IServiceCollectionQuartzConfigurator> Configurator { get; set; }

    public TimeSpan StartDelay { get; set; }

    [NotNull]
    public Func<IScheduler, Task> StartSchedulerFactory
    {
        get => _startSchedulerFactory;
        set => _startSchedulerFactory = value ?? throw new Exception(nameof(value));
    }
    private Func<IScheduler, Task> _startSchedulerFactory;

    public QuartzOptions()
    {
        Properties = new NameValueCollection();
        StartDelay = new TimeSpan(0);
        _startSchedulerFactory = StartSchedulerAsync;
    }

    private async Task StartSchedulerAsync(IScheduler scheduler)
    {
        if (StartDelay.Ticks > 0)
        {
            await scheduler.StartDelayed(StartDelay);
        }
        else
        {
            await scheduler.Start();
        }
    }
}
