using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server
{
    public class ServerSourceProviderConfiguration : ISourceProviderConfiguration
    {
        public ServerSourceProviderConfiguration(IServiceRegistry registry) { _registry = registry; }

        IServiceRegistry _registry { get; set; }

        public virtual IServiceRegistry AddSourceProvider(StoreProvider provider)
        {
            if (!DataStoreRegistry.SourceProviders.ContainsKey((int)provider))
            {
                switch (provider)
                {
                    case StoreProvider.SqlServer:
                        _registry.AddEntityFrameworkSqlServer();
                        break;
                    case StoreProvider.AzureSql:
                        _registry.AddEntityFrameworkSqlServer();
                        break;
                    case StoreProvider.PostgreSql:
                        _registry.AddEntityFrameworkNpgsql();
                        break;
                    case StoreProvider.SqlLite:
                        _registry.AddEntityFrameworkSqlite();
                        break;
                    case StoreProvider.MariaDb:
                        _registry.AddEntityFrameworkMySql();
                        break;
                    case StoreProvider.MySql:
                        _registry.AddEntityFrameworkMySql();
                        break;
                    case StoreProvider.Oracle:
                        _registry.AddEntityFrameworkOracle();
                        break;
                    case StoreProvider.CosmosDb:
                        _registry.AddEntityFrameworkCosmos();
                        break;
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
                case StoreProvider.SqlServer:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();
                case StoreProvider.AzureSql:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();

                case StoreProvider.PostgreSql:
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
                    return builder.UseInternalServiceProvider(
                         _registry.Manager)
                        .UseNpgsql(connectionString);
                        //.UseLazyLoadingProxies();

                case StoreProvider.SqlLite:
                    return builder
                        .UseSqlite(connectionString)
                        .UseLazyLoadingProxies();

                case StoreProvider.MariaDb:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case StoreProvider.MySql:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case StoreProvider.Oracle:
                    return builder
                        .UseOracle(connectionString)
                        .UseLazyLoadingProxies();

                case StoreProvider.CosmosDb:
                    return builder
                        .UseCosmos(
                            connectionString.Split('#')[0],
                            connectionString.Split('#')[1],
                            connectionString.Split('#')[2])
                        .UseLazyLoadingProxies();

                case StoreProvider.MemoryDb:
                    return builder.UseInternalServiceProvider(new ServiceManager())
                        .UseInMemoryDatabase(connectionString)
                        .UseLazyLoadingProxies()
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
