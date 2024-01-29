using Microsoft.Extensions.Hosting;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Hosting;

namespace Undersoft.SDK.Service.Application.Hosting;

public class ApplicationHost : Identifiable, IHost
{
    private IHost _host { get; set; }

    public ApplicationHost(IHostBuilder builder)
    {
        _host = builder.Build();
    }

    public string? HostName { get; set; }

    public int Port { get; set; }

    public string? ApplicationName { get; set; }

    public string? Route { get; set; }

    public long TenantId { get; set; }

    public string? TenantName { get; set; }

    public IServiceProvider? Services => _host?.Services;

    public Task? StartAsync(CancellationToken cancellationToken = default)
    {
        return _host?.StartAsync(cancellationToken);
    }

    public Task? StopAsync(CancellationToken cancellationToken = default)
    {
        return _host?.StopAsync(cancellationToken);
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
}