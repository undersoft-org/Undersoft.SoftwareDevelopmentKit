using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Undersoft.SDK.Service.Data.Store;

using Configuration;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Repository.Source;

public class DataStoreContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext> where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var config = new ServiceConfiguration();
        var configSource = config.Source(typeof(TContext).FullName);
        var provider = config.SourceProvider(configSource);
        RepositorySourceOptionsBuilder.AddEntityFrameworkSourceProvider(provider);
        var options = RepositorySourceOptionsBuilder.BuildOptions<TContext>(provider, config.SourceConnectionString(configSource)).Options;
        return typeof(TContext).New<TContext>(options);
    }

    public TContext CreateDbContext()
    {
        if (RepositoryManager.TryGetSource<TContext>(out var source))
            return source.CreateContext<TContext>();
        return null;
    }
}