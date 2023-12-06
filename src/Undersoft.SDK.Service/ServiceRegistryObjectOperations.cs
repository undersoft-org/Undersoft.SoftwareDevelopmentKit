using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service;


public partial class ServiceRegistry
{
    public ServiceRegistryObject<T> TryAddObject<T>() where T : class
    {
        if (ContainsKey(typeof(ServiceRegistryObject<T>)))
        {
            return (ServiceRegistryObject<T>)Get<ServiceRegistryObject<T>>()?.ImplementationInstance;
        }

        return AddObject<T>();
    }

    public ServiceRegistryObject TryAddObject(Type type)
    {
        Type accessorType = typeof(ServiceRegistryObject<>).MakeGenericType(type);
        if (ContainsKey(accessorType))
        {
            return (ServiceRegistryObject)Get(accessorType)?.ImplementationInstance;
        }

        return AddObject(type);
    }

    public ServiceRegistryObject<T> AddObject<T>() where T : class
    {
        return AddObject(new ServiceRegistryObject<T>());
    }

    public ServiceRegistryObject AddObject(Type type)
    {
        Type oaType = typeof(ServiceRegistryObject<>).MakeGenericType(type);
        Type iaType = typeof(IServiceRegistryObject<>).MakeGenericType(type);

        ServiceRegistryObject accessor = (ServiceRegistryObject)oaType.New();

        if (ContainsKey(oaType))
        {
            return accessor;
        }

        Put(ServiceDescriptor.Singleton(oaType), accessor);
        Put(ServiceDescriptor.Singleton(iaType), accessor);

        return accessor;
    }

    public ServiceRegistryObject AddObject(Type type, object obj)
    {
        Type oaType = typeof(ServiceRegistryObject<>).MakeGenericType(type);
        Type iaType = typeof(IServiceRegistryObject<>).MakeGenericType(type);

        ServiceRegistryObject accessor = (ServiceRegistryObject)oaType.New(obj);

        if (ContainsKey(oaType))
        {
            return accessor;
        }

        Put(ServiceDescriptor.Singleton(oaType), accessor);
        Put(ServiceDescriptor.Singleton(iaType), accessor);

        if (obj != null)
            this.AddSingleton(type, obj);

        return accessor;
    }

    public ServiceRegistryObject<T> AddObject<T>(T obj) where T : class
    {
        return AddObject(new ServiceRegistryObject<T>(obj));
    }

    public ServiceRegistryObject<T> AddObject<T>(ServiceRegistryObject<T> accessor) where T : class
    {
        if (ContainsKey(typeof(ServiceRegistryObject<T>)))
        {
            return accessor;
        }

        Put(ServiceDescriptor.Singleton(typeof(ServiceRegistryObject<T>), accessor));
        Put(ServiceDescriptor.Singleton(typeof(IServiceRegistryObject<T>), accessor));

        if (accessor.Value != null)
            this.AddSingleton<T>(accessor.Value);

        return accessor;
    }

    public object GetObject(Type type)
    {
        Type accessorType = typeof(IServiceRegistryObject<>).MakeGenericType(type);
        return ((ServiceRegistryObject)GetSingleton(accessorType))?.Value;
    }

    public T GetObject<T>()
        where T : class
    {
        return GetSingleton<IServiceRegistryObject<T>>()?.Value;
    }

    public T GetRequiredObject<T>()
        where T : class
    {
        return GetObject<T>() ?? throw new Exception($"Could not find an object of {typeof(T).AssemblyQualifiedName} in  Be sure that you have used AddObjectAccessor before!");
    }
}