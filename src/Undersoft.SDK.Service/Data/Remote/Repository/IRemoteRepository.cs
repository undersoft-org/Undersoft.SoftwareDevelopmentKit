using Microsoft.OData.Client;
using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Repository;

namespace Undersoft.SDK.Service.Data.Remote.Repository;

public interface IRemoteRepository<TStore, TEntity> : IRemoteRepository<TEntity> where TEntity : class, IOrigin, IInnerProxy
{
}

public interface IRemoteRepository<TEntity> : IRepository<TEntity> where TEntity : class, IOrigin, IInnerProxy
{
    OpenDataContext Context { get; }
    string FullName { get; }
    string Name { get; }

    Task<IEnumerable<TEntity>> FindMany(params object[] keys);
    DataServiceQuery<TEntity> FindQuery(params object[] keys);
    DataServiceQuerySingle<TEntity> FindQuerySingle(params object[] keys);

    string KeyString(params object[] keys);
    void SetAuthorizationToken(string token);
    object TracePatching(object item, string propertyName = null, Type type = null);

    Task<IEnumerable<TEntity>> Access(string method, Arguments arguments);
    Task<IEnumerable<TEntity>> Action(string method, Arguments arguments);
    Task<IEnumerable<TEntity>> Setup(string method, Arguments arguments);

    Task<IEnumerable<TEntity>> Access<TService>(Expression<Func<TService, Delegate>> method, Arguments arguments);
    Task<IEnumerable<TEntity>> Action<TService>(Expression<Func<TService, Delegate>> method, Arguments arguments);
    Task<IEnumerable<TEntity>> Setup<TService>(Expression<Func<TService, Delegate>> method, Arguments arguments);
}