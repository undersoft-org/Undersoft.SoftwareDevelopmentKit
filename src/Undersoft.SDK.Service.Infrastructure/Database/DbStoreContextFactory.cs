using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Undersoft.SDK.Service.Data.Store;

using Configuration;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Repository;

public class DbStoreContextFactory<TContext, TSourceProvider> : IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext> where TContext : DbContext where TSourceProvider : class, ISourceProviderConfiguration
{
    public TContext CreateDbContext(string[] args)
    {
        var config = new ServiceConfiguration();
        var configSource = config.Source(typeof(TContext).FullName);
        var provider = config.SourceProvider(configSource);
        StoreSourceOptionsBuilder.AddRootEntityFrameworkSourceProvider<TSourceProvider>(provider);
        var options = StoreSourceOptionsBuilder.BuildOptions<TContext>(provider, config.SourceConnectionString(configSource)).Options;
        return typeof(TContext).New<TContext>(options);
    }

    public TContext CreateDbContext()
    {
        return this.CreateDbContext(null);
    }
}