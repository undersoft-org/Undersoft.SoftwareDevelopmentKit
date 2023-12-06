using Microsoft.Extensions.DependencyInjection;

namespace Undersoft.SDK.Service
{
    using Configuration;
    using Undersoft.SDK.Service.Data.Repository;

    public interface IServiceManager : IRepositoryManager, IServiceProvider
    {
        IServiceRegistry Registry { get; }
        IServiceConfiguration Configuration { get; set; }
        IServiceProvider RootProvider { get; }
        IServiceScope Session { get; }

        T EnsureGetRootService<T>() where T : class;
        object GetRequiredService(Type type);
        T GetRequiredService<T>() where T : class;
        Lazy<T> GetRequiredServiceLazy<T>() where T : class;
        T GetService<T>()where T : class;
        object GetSingleton(Type type);
        T GetSingleton<T>() where T : class;
        Lazy<T> GetServiceLazy<T>() where T : class;
        IEnumerable<object> GetServices(Type type);
        IEnumerable<T> GetServices<T>() where T : class;
        Lazy<IEnumerable<T>> GetServicesLazy<T>() where T : class;
        ObjectFactory NewFactory(Type instanceType, Type[] constrTypes);
        ObjectFactory NewFactory<T>(Type[] constrTypes);
        T NewRootService<T>(params object[] parameters) where T : class;
    }
}