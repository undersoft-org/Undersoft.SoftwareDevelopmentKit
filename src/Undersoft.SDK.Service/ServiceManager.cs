using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Undersoft.SDK.Service
{
    using Configuration;
    using Undersoft.SDK.Service.Data.Repository;

    public class ServiceManager : RepositoryManager, IServiceManager, IAsyncDisposable
    {
        private new bool disposedValue;
        private static IServiceRegistry registry;
        private static IServiceConfiguration configuration;

        protected IServiceScope session;
        protected IServiceProvider provider;

        public IServiceProvider RootProvider => GetRootProvider();
        public IServiceProvider Provider =>  provider ??= GetProvider();
        public IServiceScope Session => session ??= GetSession();

        public IServiceConfiguration Configuration
        {
            get => configuration;
            set => configuration = value;
        }
        public IServiceRegistry Registry => registry;

        public ServiceManager() : base()
        {
            Services = this;
        }

        internal ServiceManager(IServiceCollection services) : this()
        {
            if (registry == null)
            {
                registry = new ServiceRegistry(services, this);
                registry.MergeServices();
                AddObject<IServiceManager>(this);
                BuildServiceProviderFactory(registry);
            }
            else
                registry.MergeServices(services, false);

            if (configuration == null)
            {
                configuration = new ServiceConfiguration(registry);
                AddObject<IServiceConfiguration>(configuration);
            }
        }

        public virtual IServiceProviderFactory<IServiceCollection> BuildServiceProviderFactory(IServiceRegistry registry)
        {
            var options = new ServiceProviderOptions();

            var factory = new DefaultServiceProviderFactory(options);

            AddObject<IServiceProviderFactory<IServiceCollection>>(factory);
            AddObject<IServiceCollection>(registry);

            registry.Services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(factory));
            registry.Services.Replace(ServiceDescriptor.Singleton<IServiceCollection>(registry));
            registry.MergeServices();

            return factory;
        }

        public virtual T GetRootService<T>() where T : class
        {
            var result = RootProvider.GetService<T>();
            return result;
        }

        public virtual IEnumerable<T> GetRootServices<T>() where T : class
        {
            return RootProvider.GetServices<T>();
        }

        public virtual T GetRequiredRootService<T>() where T : class
        {
            return RootProvider.GetRequiredService<T>();
        }

        public virtual object GetRootService(Type type)
        {
            return RootProvider.GetService(type);
        }

        public virtual T GetService<T>() where T : class
        {
            var result = Provider.GetService<T>();
            return result;
        }

        public virtual IEnumerable<T> GetServices<T>() where T : class
        {
            return Provider.GetServices<T>();
        }

        public virtual T GetRequiredService<T>() where T : class
        {
            return Provider.GetRequiredService<T>();
        }

        public virtual object GetService(Type type)
        {
            return Provider.GetService(type);
        }

        public virtual IEnumerable<object> GetServices(Type type)
        {
            return Provider.GetServices(type);
        }

        public Lazy<T> GetRequiredServiceLazy<T>() where T : class
        {
            return new Lazy<T>(GetRequiredService<T>, true);
        }

        public Lazy<T> GetServiceLazy<T>() where T : class
        {
            return new Lazy<T>(GetService<T>, true);
        }

        public Lazy<IEnumerable<T>> GetServicesLazy<T>() where T : class
        {
            return new Lazy<IEnumerable<T>>(GetServices<T>, true);
        }

        public virtual T GetSingleton<T>() where T : class
        {
            return GetObject<T>();
        }

        public virtual object GetSingleton(Type type)
        {
            return registry.GetObject(type);
        }

        public virtual object GetRequiredService(Type type)
        {
            return Provider.GetRequiredService(type);
        }

        public virtual T NewRootService<T>(params object[] parameters) where T : class
        {
            return ActivatorUtilities.CreateInstance<T>(RootProvider, parameters);
        }

        public virtual T EnsureGetRootService<T>() where T : class
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(RootProvider);
        }

        public static void SetProvider(IServiceProvider serviceProvider)
        {
            var _provider = serviceProvider;
            _provider.GetRequiredService<ServiceRegistryObject<IServiceProvider>>().Value = _provider;
        }

        public static IServiceProvider BuildInternalProvider()
        {
            var provider = GetRegistry().BuildServiceProviderFromFactory<IServiceCollection>();
            SetProvider(provider);
            return provider;
        }

        public static IServiceProvider GetRootProvider()
        {
            var _provider = registry.GetProvider();
            if (_provider == null)
                return BuildInternalProvider();

            return _provider;
        }

        public IServiceProvider GetProvider()
        {
            if (provider == null)
                provider = Session.ServiceProvider;
            return provider;
        }

        public static IServiceProviderFactory<IServiceCollection> GetProviderFactory()
        {
            return GetObject<IServiceProviderFactory<IServiceCollection>>();
        }

        public ObjectFactory NewFactory<T>(Type[] constrTypes)
        {
            return ActivatorUtilities.CreateFactory(typeof(T), constrTypes);
        }

        public ObjectFactory NewFactory(Type instanceType, Type[] constrTypes)
        {
            return ActivatorUtilities.CreateFactory(instanceType, constrTypes);
        }

        public static T GetObject<T>() where T : class
        {
            return registry.GetObject<T>();
        }

        public static T AddObject<T>(T obj) where T : class
        {
            return registry.AddObject(obj).Value;
        }
        public static T AddObject<T>() where T : class
        {
            return registry.AddObject(typeof(T).New<T>()).Value;
        }

        public IServiceScope GetSession()
        {
            if(session == null)
                session = NewSession();
            return session;
        }

        public IServiceScope NewSubSession()
        {
            return GetProvider().CreateScope();
        }

        public static IServiceScope NewSession()
        {
            return GetRootProvider().CreateScope();
        }

        public static IServiceManager GetManager()
        {
            if (registry == null)
                return new ServiceManager(new ServiceCollection());
            return GetObject<IServiceManager>();
        }

        public static IServiceRegistry GetRegistry()
        {
            if (registry == null)
                return new ServiceManager(new ServiceCollection()).Registry;
            return registry;
        }

        public static IServiceConfiguration GetConfiguration()
        {
            return configuration;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (session != null)
                        session.Dispose();
                }
                disposedValue = true;
            }
        }

        public override async ValueTask DisposeAsyncCore()
        {
            await new ValueTask(Task.Run(() =>
            {
                if (session != null)
                    session.Dispose();

            })).ConfigureAwait(false);
        }
    }
}
