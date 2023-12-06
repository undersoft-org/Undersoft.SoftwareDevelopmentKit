using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository.Source
{
    public static class RepositorySourceOptionsBuilder
    {
        public static IServiceRegistry AddEntityFrameworkSourceProvider(SourceProvider provider)
        {
            IServiceRegistry registry = ServiceManager.GetRegistry();
            if (!DataStoreRegistry.SourceProviders.ContainsKey((int)provider))
            {
                switch (provider)
                {
                    case SourceProvider.SqlServer:
                        registry.AddEntityFrameworkSqlServer();
                        break;
                    case SourceProvider.AzureSql:
                        registry.AddEntityFrameworkSqlServer();
                        break;
                    case SourceProvider.PostgreSql:
                        registry.AddEntityFrameworkNpgsql();
                        break;
                    case SourceProvider.SqlLite:
                        registry.AddEntityFrameworkSqlite();
                        break;
                    case SourceProvider.MariaDb:
                        registry.AddEntityFrameworkMySql();
                        break;
                    case SourceProvider.MySql:
                        registry.AddEntityFrameworkMySql();
                        break;
                    case SourceProvider.Oracle:
                        registry.AddEntityFrameworkOracle();
                        break;
                    case SourceProvider.CosmosDb:
                        registry.AddEntityFrameworkCosmos();
                        break;
                    case SourceProvider.MemoryDb:
                        registry.AddEntityFrameworkInMemoryDatabase();
                        break;
                    default:
                        break;
                }
                registry.AddEntityFrameworkProxies();
                DataStoreRegistry.SourceProviders.Add((int)provider, provider);
            }
            return registry;
        }

        public static DbContextOptionsBuilder<TContext> BuildOptions<TContext>(
            SourceProvider provider,
            string connectionString)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)BuildOptions(
                new DbContextOptionsBuilder<TContext>(),
                provider,
                connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        public static DbContextOptionsBuilder BuildOptions(SourceProvider provider, string connectionString)
        {
            return BuildOptions(new DbContextOptionsBuilder(), provider, connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        public static DbContextOptionsBuilder BuildOptions(
            DbContextOptionsBuilder builder,
            SourceProvider provider,
            string connectionString)
        {
            switch (provider)
            {
                case SourceProvider.SqlServer:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();
                case SourceProvider.AzureSql:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();

                case SourceProvider.PostgreSql:
                    return builder.UseInternalServiceProvider(
                        ServiceManager.GetManager())
                        .UseNpgsql(connectionString)
                        .UseLazyLoadingProxies();

                case SourceProvider.SqlLite:
                    return builder
                        .UseSqlite(connectionString)
                        .UseLazyLoadingProxies();

                case SourceProvider.MariaDb:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case SourceProvider.MySql:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case SourceProvider.Oracle:
                    return builder
                        .UseOracle(connectionString)
                        .UseLazyLoadingProxies();

                case SourceProvider.CosmosDb:
                    return builder
                        .UseCosmos(
                            connectionString.Split('#')[0],
                            connectionString.Split('#')[1],
                            connectionString.Split('#')[2])
                        .UseLazyLoadingProxies();

                case SourceProvider.MemoryDb:
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

            return builder;
        }
    }

    public enum SourceProvider
    {
        None,
        SqlServer,
        MemoryDb,
        AzureSql,
        PostgreSql,
        SqlLite,
        MySql,
        MariaDb,
        Oracle,
        CosmosDb,
        MongoDb,
        FileSystem
    }

    public enum ClientProvider
    {
        None,
        Open,
        Crud,
        Stream
    }
}
