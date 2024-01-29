using Microsoft.OData.Client;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Remote.Repository;

using Instant.Proxies;
using Instant.Updating;
using Logging;
using Series;
using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Remote;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Repository.Client;
using Undersoft.SDK.Service.Data.Store;

public class RemoteRepository<TStore, TEntity>
    : RemoteRepository<TEntity>,
        IRemoteRepository<TStore, TEntity>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    public RemoteRepository(IRepositoryContextPool<OpenDataClient<TStore>> pool)
        : base(pool.ContextPool)
    {
        mapper = DataMapper.GetMapper();
    }

    public RemoteRepository(
        IRepositoryContextPool<OpenDataClient<TStore>> pool,
        IEntityCache<TStore, TEntity> cache
    ) : base(pool.ContextPool)
    {
        mapper = cache.Mapper;
        this.cache = cache;
    }

    public RemoteRepository(
        IRepositoryContextPool<OpenDataClient<TStore>> pool,
        IEntityCache<TStore, TEntity> cache,
        IAuthorization authorization
    ) : base(pool.ContextPool)
    {
        remoteContext.SetAuthorizationHeader(authorization.Credentials.SessionToken);
        mapper = cache.Mapper;
        this.cache = cache;
    }

    public override Task<int> Save(bool asTransaction, CancellationToken token = default)
    {
        return ContextLease.Save(asTransaction, token);
    }
}

public partial class RemoteRepository<TEntity> : Repository<TEntity>, IRemoteRepository<TEntity>
    where TEntity : class, IOrigin, IInnerProxy
{
    ISeries<DataServiceRequest> _batchset;
    protected OpenDataContext remoteContext => (OpenDataContext)InnerContext;
    protected DataServiceQuery<TEntity> remoteQuery;
    protected RemoteSet<TEntity> remoteSet;

    public RemoteRepository() { }

    public RemoteRepository(IRepositoryClient repositorySource) : base(repositorySource)
    {
        if (remoteContext != null)
        {
            remoteSet = new RemoteSet<TEntity>(
                remoteContext,
                remoteContext.GetMappedName(typeof(TEntity)),
                null,
                null
            );
            remoteQuery = remoteSet.Query;
            Expression = Expression.Constant(this);
            Provider = new RemoteRepositoryExpressionProvider<TEntity>(Query);
        }
    }

    public RemoteRepository(OpenDataContext context) : base(context)
    {
        if (remoteContext != null)
        {
            remoteSet = new RemoteSet<TEntity>(
                remoteContext,
                remoteContext.GetMappedName(typeof(TEntity)),
                null,
                null
            );
            remoteQuery = remoteSet.Query;
            Expression = Expression.Constant(this.AsEnumerable());
            Provider = new RemoteRepositoryExpressionProvider<TEntity>(remoteQuery);
        }
    }

    public RemoteRepository(IRepositoryContextPool context) : base(context)
    {
        if (remoteContext != null)
        {
            remoteSet = new RemoteSet<TEntity>(
                remoteContext,
                remoteContext.GetMappedName(typeof(TEntity)),
                null,
                null
            );
            remoteQuery = remoteSet.Query;
            Expression = Expression.Constant(this);
            Provider = new RemoteRepositoryExpressionProvider<TEntity>(Query);
        }
    }

    public RemoteRepository(IQueryProvider provider, Expression expression)
    {
        ElementType = typeof(TEntity).GetDataType();
        Provider = provider;
        Expression = expression;
    }

    public override TEntity this[params object[] keys]
    {
        get => find(keys);
        set
        {
            TEntity entity = find(keys);
            if (entity != null)
            {
                value.PatchTo(entity, PatchingEvent);
                //dsContext.UpdateObject(Stamp(entity), keys);
                if (remoteSet.ContainsKey(keys))
                    remoteSet[keys] = Stamp(entity);
                else
                    remoteSet.Add(Stamp(entity));
            }
            else
                remoteSet.Add(Sign(entity));
            //dsContext.AddObject(Name, Sign(value));
        }
    }

    public override TEntity this[object[] keys, params Expression<
        Func<TEntity, object>
    >[] expanders]
    {
        get
        {
            DataServiceQuery<TEntity> query = remoteContext.CreateQuery<TEntity>(KeyString(keys), true);
            if (expanders != null)
                foreach (Expression<Func<TEntity, object>> expander in expanders)
                    query = query.Expand(expander);

            var entity = query.FirstOrDefault();
            if (entity != null)
                if (remoteSet.ContainsKey(keys))
                    remoteSet[keys] = entity;
                else
                    remoteSet.Add(entity);
            return entity;
        }
        set
        {
            DataServiceQuery<TEntity> query = remoteContext.CreateQuery<TEntity>(KeyString(keys), true);
            if (expanders != null)
                if (expanders != null)
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                        query = query.Expand(expander);

            TEntity entity = query.FirstOrDefault();
            if (entity != null)
            {
                value.PatchTo(Stamp(entity), PatchingEvent);
                if (remoteSet.ContainsKey(keys))
                    remoteSet[keys] = Stamp(entity);
                else
                    remoteSet.Add(Stamp(entity));
            }
        }
    }

    public override object this[Expression<
        Func<TEntity, object>
    > selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders]
    {
        get
        {
            DataServiceQuery<TEntity> query = FindQuery(keys);
            if (expanders != null)
                foreach (Expression<Func<TEntity, object>> expander in expanders)
                    query = query.Expand(expander);

            remoteSet.Load(query);
            return remoteSet[keys].ToQueryable().Select(selector).FirstOrDefault();
        }
        set
        {
            DataServiceQuery<TEntity> query = FindQuery(keys);
            if (expanders != null)
                foreach (Expression<Func<TEntity, object>> expander in expanders)
                    query = query.Expand(expander);

            remoteSet.Load(query);
            remoteSet.AsQueryable().Select(selector).FirstOrDefault().PatchFrom(value);
        }
    }

    private TEntity lookup(params object[] keys)
    {
        var item = cache.Lookup<TEntity>(keys);
        if (item != null)
            remoteSet.Load(item);
        else
            item = remoteSet[keys];
        return item;
    }

    TEntity find(params object[] keys)
    {
        if (keys != null)
        {
            return lookup(keys);
        }
        return null;
    }

    Task<TEntity> findAsync(params object[] keys)
    {
        return Task.Run(() =>
        {
            if (keys != null)
            {
                return lookup(keys);
            }
            return null;
        });
    }

    DataServiceRequest findOne(params object[] keys)
    {
        if (keys != null)
        {
            if (_batchset == null)
                _batchset = new Registry<DataServiceRequest>();

            return _batchset.Put(keys, remoteContext.CreateQuery<TEntity>(KeyString(keys), true)).Value;
        }
        return null;
    }

    protected override TEntity InnerSet(TEntity entity)
    {
        //dsContext.UpdateObject(Stamp(entity));
        if (entity != null)
        {
            var id = Stamp(entity).Id;
            var _entity = remoteSet[id];
            if (_entity != null)
                entity.PatchTo(_entity);
            return _entity;
        }
        return null;
    }

    protected override async Task<int> saveAsTransaction(CancellationToken token = default)
    {
        try
        {
            return (
                await remoteContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset, token)
            ).Count();
        }
        catch (Exception e)
        {
            remoteContext.Failure<Datalog>(
                $"{$"Fail on update dataservice as singlechangeset, using context:{remoteContext.GetType().Name}, "}{$"TimeStamp: {DateTime.Now.ToString()}"}",
                ex: e
            );
        }

        return -1;
    }

    protected override async Task<int> saveChanges(CancellationToken token = default)
    {
        try
        {
            return (
                await remoteContext.SaveChangesAsync(
                    SaveChangesOptions.BatchWithIndependentOperations
                        | SaveChangesOptions.ContinueOnError,
                    token
                )
            ).Count();
        }
        catch (Exception e)
        {
            remoteContext.Failure<Datalog>(
                $"{$"Fail on update dataservice as independent operations, using context:{remoteContext.GetType().Name}, "}{$"TimeStamp: {DateTime.Now.ToString()}"}",
                ex: e
            );
        }

        return -1;
    }    

    public override object TracePatching(object item, string propertyName = null, Type type = null)
    {
        if (type == null)
        {
            remoteContext.AttachTo(item.GetType().Name, item);
            return item;
        }
        else
        {
            var qor = remoteContext.LoadProperty(item, propertyName);
            var source = (IProxy)item;
            var target = source[propertyName];
            if (target == null)
                source[propertyName] = target = type.New();
            return target;
        }
    }

    public override TEntity Add(TEntity entity)
    {
        TEntity _entity = Sign(entity);
        remoteSet.Add(_entity);
        return entity;
    }

    public override IEnumerable<TEntity> Add(IEnumerable<TEntity> entity)
    {
        foreach (TEntity e in entity)
            yield return Add(e);
    }

    public override IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entity)
    {
        return entity.ForEachAsync((e) => Add(e));
    }

    public override TEntity Delete(TEntity entity)
    {
        if (remoteSet.ContainsKey(entity.Id))
            remoteSet.Remove(entity);
        else
            remoteContext.DeleteObject(entity);
        return entity;
    }

    public override Task<TEntity> Find(params object[] keys)
    {
        return findAsync(keys);
    }

    public async Task<IEnumerable<TEntity>> FindMany(params object[] keys)
    {
        foreach (object key in keys)
        {
            if (key.GetType().IsAssignableTo(typeof(object[])))
                findOne((object[])key);
            else
                findOne(key);
        }
        return (await Context.ExecuteBatchAsync(_batchset.ToArray())).SelectMany(
            o => o as QueryOperationResponse<TEntity>
        );
    }

    public string KeyString(params object[] keys)
    {
        return $"{Name}({(keys.Length > 1 ? keys.Aggregate(string.Empty, (a, b) => $"{a},{b}") : keys[0])})";
    }

    public override TEntity NewEntry(params object[] parameters)
    {
        TEntity entity = Sign(typeof(TEntity).New<TEntity>(parameters));
        remoteContext.AddObject(Name, entity);
        return entity;
    }

    public DataServiceQuerySingle<TEntity> FindQuerySingle(params object[] keys)
    {
        if (keys != null)
            return remoteQuery.CreateFunctionQuerySingle<TEntity>(KeyString(keys), true);
        return null;
    }

    public DataServiceQuery<TEntity> FindQuery(params object[] keys)
    {
        if (keys != null)
            return remoteQuery.CreateFunctionQuery<TEntity>(KeyString(keys), true);
        return null;
    }

    public override IQueryable<TEntity> AsQueryable()
    {
        return Query;
    }

    public OpenDataContext Context => remoteContext;

    public override string Name => Context.GetMappedName(ElementType);

    public override string FullName => Context.GetMappedFullName(ElementType);

    public override DataServiceQuery<TEntity> Query => remoteContext.CreateQuery<TEntity>(Name, true);

    public void SetAuthorizationToken(string token)
    {
        remoteContext.SetAuthorizationHeader(token);
    }
}
