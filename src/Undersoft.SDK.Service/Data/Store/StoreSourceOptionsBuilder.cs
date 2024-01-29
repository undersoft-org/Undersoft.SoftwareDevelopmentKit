using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Undersoft.SDK.Service.Data.Repository.Source;

namespace Undersoft.SDK.Service.Data.Store
{
    public static class StoreSourceOptionsBuilder
    {
        public static void AddRootEntityFrameworkSourceProvider<TSourceProvider>(StoreProvider provider) where TSourceProvider : class, ISourceProviderConfiguration
        {
            var sourceConfiguration = typeof(TSourceProvider).New<TSourceProvider>(ServiceManager.GetRootManager().Registry);
            sourceConfiguration.AddSourceProvider(provider);
            ServiceManager.AddRootObject<ISourceProviderConfiguration>(sourceConfiguration);
        }

        public static ISourceProviderConfiguration AddEntityFrameworkSourceProvider(StoreProvider provider)
        {
            var sourceConfiguration = ServiceManager.GetRootObject<ISourceProviderConfiguration>();
            sourceConfiguration.AddSourceProvider(provider);
            return sourceConfiguration;
        }

        public static ISourceProviderConfiguration AddEntityFrameworkSourceProvider(this IServiceRegistry registry, StoreProvider provider)
        {
            var sourceConfiguration = registry.GetObject<ISourceProviderConfiguration>();
            sourceConfiguration.AddSourceProvider(provider);
            return sourceConfiguration;
        }

        public static DbContextOptionsBuilder<TContext> BuildOptions<TContext>(
            StoreProvider provider,
            string connectionString)
            where TContext : DbContext
        {
            var builder = ServiceManager.GetRootObject<ISourceProviderConfiguration>();

            return (DbContextOptionsBuilder<TContext>)builder.BuildOptions(
                new DbContextOptionsBuilder<TContext>(),
                provider,
                connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        public static DbContextOptionsBuilder BuildOptions(StoreProvider provider, string connectionString)
        {
            var builder = ServiceManager.GetRootObject<ISourceProviderConfiguration>();

            return builder.BuildOptions(new DbContextOptionsBuilder(), provider, connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }
    }
}
