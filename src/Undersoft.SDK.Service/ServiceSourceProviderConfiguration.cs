using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service
{
    public class ServiceSourceProviderConfiguration : ISourceProviderConfiguration
    {
        public ServiceSourceProviderConfiguration() { }

        public ServiceSourceProviderConfiguration(IServiceRegistry registry) { _registry = registry; }

        IServiceRegistry _registry { get; set; }
        
        public virtual IServiceRegistry AddSourceProvider(StoreProvider provider)
        {            
            if (!DataStoreRegistry.SourceProviders.ContainsKey((int)provider))
            {
                switch (provider)
                {
                    case StoreProvider.MemoryDb:
                        _registry.AddEntityFrameworkInMemoryDatabase();
                        break;
                    default:
                        break;
                }
                //_registry.AddEntityFrameworkProxies();
                DataStoreRegistry.SourceProviders.Add((int)provider, provider);
            }
            return _registry;
        }

        public virtual DbContextOptionsBuilder BuildOptions(
         DbContextOptionsBuilder builder,
         StoreProvider provider,
         string connectionString)
        {
            switch (provider)
            {
                case StoreProvider.MemoryDb:
                    return builder.UseInternalServiceProvider(new ServiceManager())
                        .UseInMemoryDatabase(connectionString)
                        //.UseLazyLoadingProxies()
                        .ConfigureWarnings(
                            w => w.Ignore(
                                InMemoryEventId
                                .TransactionIgnoredWarning));
                default:
                    break;
            }

            builder.ConfigureWarnings(warnings => warnings
                    .Ignore(CoreEventId.RedundantIndexRemoved));
            return builder;
        }
    }
}
