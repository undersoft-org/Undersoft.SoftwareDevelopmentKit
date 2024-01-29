using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Undersoft.SDK.Service.Configuration;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository.Source;

public class RepositorySource<TContext> : RepositorySource, IRepositorySource<TContext>
    where TContext : DbContext, IDataStoreContext
{
    protected new DbContextOptionsBuilder<TContext> optionsBuilder;

    public RepositorySource() : base()
    {
    }

    public RepositorySource(IServiceConfiguration config) : base()
    {
        contextType = typeof(TContext);
        IConfigurationSection endpoint = config.Source(contextType.FullName);
        string connectionString = config.SourceConnectionString(contextType.FullName);
        StoreProvider provider = config.SourceProvider(contextType.FullName);
        ContextPool = this;
        PoolSize = config.SourcePoolSize(endpoint);
        optionsBuilder = StoreSourceOptionsBuilder.BuildOptions<TContext>(provider, connectionString);
        InnerContext = CreateContext(optionsBuilder.Options);
        Context.GetEntityTypes();
    }

    public RepositorySource(DbContextOptions<TContext> options) : base()
    {
        ContextPool = this;
        contextType = options.ContextType;
        InnerContext = CreateContext(options);
        Context.GetEntityTypes();
    }

    public RepositorySource(IRepositorySource pool) : base(pool)
    {
    }

    public RepositorySource(StoreProvider provider, string connectionString) : base()
    {
        ContextPool = this;
        contextType = typeof(TContext);
        optionsBuilder = StoreSourceOptionsBuilder.BuildOptions<TContext>(provider, connectionString);
        InnerContext = CreateContext(optionsBuilder.Options);
        Context.GetEntityTypes();
    }

    public override TContext Context => (TContext)InnerContext;

    public override DbContextOptions<TContext> Options => (DbContextOptions<TContext>)base.Options;

    public override TContext CreateContext() { return typeof(TContext).New<TContext>(Options); }

    public TContext CreateContext(DbContextOptions<TContext> options)
    {
        Options ??= options;
        Type type = typeof(TContext);
        contextType ??= type;
        return type.New<TContext>(options);
    }

    public TContext CreateDbContext() { return CreateContext(); }
    public TContext CreateDbContext(string[] args) { return CreateContext(); }
}
