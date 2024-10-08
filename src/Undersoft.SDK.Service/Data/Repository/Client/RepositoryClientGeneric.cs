namespace Undersoft.SDK.Service.Data.Repository.Client;

using Configuration;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Utilities;

public class RepositoryClient<TContext> : RepositoryClient, IRepositoryClient<TContext> where TContext : DataClientContext
{
    public RepositoryClient() { }
    public RepositoryClient(IServiceConfiguration config) : this()
    {
        var contextType = typeof(TContext);
        var endpoint = config.Client(contextType.FullName);
        var connectionString = config.ClientConnectionString(contextType.FullName);
        PoolSize = config.ClientPoolSize(endpoint);
        InnerContext = CreateContext(contextType, new Uri(connectionString));
    }
    public RepositoryClient(string connectionString)
    {
        ContextPool = this;
        contextType = typeof(TContext);
        InnerContext = CreateContext<TContext>(new Uri(connectionString));
    }
    public RepositoryClient(Uri serviceRoot)
    {
        ContextPool = this;
        contextType = typeof(TContext);
        InnerContext = CreateContext<TContext>(uri);
    }
    public RepositoryClient(IRepositoryClient client) : base(client)
    {
    }

    public override TContext Context => (TContext)InnerContext;

    public override TContext CreateContext()
    {
        return typeof(TContext).New<TContext>(uri);
    }

    public TContext CreateContext(Uri serviceRoot)
    {
        uri = serviceRoot;
        return typeof(TContext).New<TContext>(uri);
    }
}
