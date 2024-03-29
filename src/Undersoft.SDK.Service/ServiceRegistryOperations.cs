namespace Undersoft.SDK.Service
{
    using Microsoft.Extensions.DependencyInjection;
    using Undersoft.SDK.Logging;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    public partial class ServiceRegistry
    {
        public IServiceProvider BuildServiceProviderFromFactory()
        {
            foreach (var service in Services)
            {
                var factoryInterface = service.ImplementationInstance
                    ?.GetType()
                    .GetTypeInfo()
                    .GetInterfaces()
                    .FirstOrDefault(
                        i =>
                            i.GetTypeInfo().IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IServiceProviderFactory<>)
                    );

                if (factoryInterface == null)
                {
                    continue;
                }

                var containerBuilderType = factoryInterface.GenericTypeArguments[0];
                return (IServiceProvider)
                    typeof(ServiceRegistry)
                        .GetTypeInfo()
                        .GetMethods()
                        .Single(
                            m =>
                                m.Name == nameof(BuildServiceProviderFromFactory)
                                && m.IsGenericMethod
                        )
                        .MakeGenericMethod(containerBuilderType)
                        .Invoke(null, new object[] { this, null });
            }

            return this.BuildServiceProvider();
        }

        public IServiceProvider BuildServiceProviderFromFactory<TContainerBuilder>(
            [NotNull] Action<TContainerBuilder> builderAction = null
        )
        {
            var serviceProviderFactory = GetSingleton<IServiceProviderFactory<TContainerBuilder>>();
            if (serviceProviderFactory == null)
            {
                Log.Failure<Datalog, Exception>(
                    null,
                    $"Could not find {typeof(IServiceProviderFactory<TContainerBuilder>).FullName} in {this}."
                );
            }

            var builder = serviceProviderFactory.CreateBuilder(this);
            builderAction?.Invoke(builder);
            return serviceProviderFactory.CreateServiceProvider(builder);
        }

        public T GetRequiredService<T>() where T : class
        {
            return GetSingleton<IServiceManager>().Provider.GetRequiredService<T>();
        }

        public object GetRequiredService(Type type)
        {
            return GetSingleton<IServiceManager>().Provider.GetRequiredService(type);
        }

        public Lazy<T> GetRequiredServiceLazy<T>() where T : class
        {
            return new Lazy<T>(GetRequiredService<T>, true);
        }

        public Lazy<object> GetRequiredServiceLazy(Type type)
        {
            return new Lazy<object>(() => GetRequiredService(type), true);
        }

        public Lazy<T> GetServiceLazy<T>() where T : class
        {
            return new Lazy<T>(GetService<T>, true);
        }

        public Lazy<object> GetServiceLazy(Type type)
        {
            return new Lazy<object>(() => GetService(type), true);
        }

        public IServiceProvider GetProvider()
        {
            return (
                (IServiceObject<IServiceProvider>)(
                    Get<IServiceObject<IServiceProvider>>()?.ImplementationInstance
                )
            ).Value;
        }

        public T GetRequiredSingleton<T>() where T : class
        {
            var service = GetSingleton<T>();
            if (service == null)
            {
                throw new InvalidOperationException(
                    "Could not find singleton service: " + typeof(T).AssemblyQualifiedName
                );
            }

            return service;
        }

        public T GetSingleton<T>() where T : class
        {
            return (T)Get<T>()?.ImplementationInstance;
        }

        public object GetSingleton(Type type)
        {
            return Get(type)?.ImplementationInstance;
        }

        public bool IsAdded<T>() where T : class
        {
            return ContainsKey<T>();
        }

        public bool IsAdded(Type type)
        {
            return ContainsKey(type);
        }

        internal T GetService<T>() where T : class
        {
            return GetSingleton<IServiceManager>().Provider.GetService<T>();
        }

        internal object GetService(Type type)
        {
            return GetSingleton<IServiceManager>().Provider.GetService(type);
        }
    }
}
