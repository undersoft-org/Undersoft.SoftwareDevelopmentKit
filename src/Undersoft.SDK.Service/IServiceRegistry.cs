﻿using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service
{
    public interface IServiceRegistry : IServiceCollection
    {
        ServiceDescriptor this[string name] { get; set; }
        ServiceDescriptor this[Type serviceType] { get; set; }

        IServiceManager Manager { get; }
        IServiceCollection Services { get; set; }

        void Add(ServiceDescriptor item);
        ServiceObject AddObject(Type type);
        ServiceObject AddObject(Type type, object obj);
        ServiceObject<T> AddObject<T>() where T : class;
        ServiceObject<T> AddObject<T>(ServiceObject<T> accessor) where T : class;
        ServiceObject<T> AddObject<T>(T obj) where T : class;
        IServiceProvider BuildServiceProviderFromFactory();
        IServiceProvider BuildServiceProviderFromFactory<TContainerBuilder>([NotNull] Action<TContainerBuilder> builderAction = null);
        bool Contains(ServiceDescriptor item);
        bool ContainsKey(Type type);
        bool ContainsKey<TService>();
        void CopyTo(ServiceDescriptor[] array, int arrayIndex);
        ServiceDescriptor Get(Type contextType);
        ServiceDescriptor Get<TService>() where TService : class;
        long GetKey(ServiceDescriptor item);
        long GetKey(string item);
        long GetKey(Type item);
        long GetKey<T>();
        object GetObject(Type type);
        T GetObject<T>() where T : class;
        IServiceProvider GetProvider();
        T GetRequiredObject<T>() where T : class;
        object GetRequiredService(Type type);
        T GetRequiredService<T>() where T : class;
        Lazy<object> GetRequiredServiceLazy(Type type);
        Lazy<T> GetRequiredServiceLazy<T>() where T : class;
        T GetRequiredSingleton<T>() where T : class;
        Lazy<object> GetServiceLazy(Type type);
        Lazy<T> GetServiceLazy<T>() where T : class;
        object GetSingleton(Type type);
        T GetSingleton<T>() where T : class;
        int IndexOf(ServiceDescriptor item);
        void Insert(int index, ServiceDescriptor item);
        bool IsAdded(Type type);
        bool IsAdded<T>() where T : class;
        void MergeServices(bool actualizeExternalServices = true);
        void MergeServices(IServiceCollection services, bool actualizeExternalServices = true);
        bool Remove(ServiceDescriptor item);
        bool Remove<TContext>() where TContext : class;
        ISeriesItem<ServiceDescriptor> Set(ServiceDescriptor descriptor);
        bool TryAdd(ServiceDescriptor profile);
        ServiceObject TryAddObject(Type type);
        ServiceObject<T> TryAddObject<T>() where T : class;
        bool TryGet<TService>(out ServiceDescriptor profile) where TService : class;
    }
}