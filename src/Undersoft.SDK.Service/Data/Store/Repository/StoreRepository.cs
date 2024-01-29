using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Store.Repository;

using Instant.Updating;
using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Repository;

public partial class StoreRepository<TEntity> : Repository<TEntity>, IStoreRepository<TEntity>
    where TEntity : class, IOrigin, IInnerProxy
{
    IQueryable<TEntity> _query;

    public StoreRepository() : base() { }

    public StoreRepository(IRepositorySource repositorySource) : base(repositorySource)
    {
        TrackingEvents();
    }

    public StoreRepository(DataStoreContext context) : base(context)
    {
        TrackingEvents();

        Expression = Expression.Constant(this.AsEnumerable());
        Provider = new StoreRepositoryExpressionProvider<TEntity>(context);
    }

    public StoreRepository(IRepositoryContextPool context) : base(context)
    {
        TrackingEvents();
    }

    public StoreRepository(IQueryProvider provider, Expression expression)
    {
        ElementType = typeof(TEntity).GetDataType();
        Provider = provider;
        Expression = expression;
    }

    protected IDataStoreContext store => (IDataStoreContext)InnerContext;

    private TEntity lookup(params object[] keys)
    {
        var item = cache.Lookup<TEntity>(keys);
        if (item != null)
            return store.Attach(item);
        else
            return store.Find<TEntity>(keys);
    }

    public override TEntity this[params object[] keys]
    {
        get { return lookup(keys); }
        set
        {
            object current = null;
            TEntity entity = lookup(keys);

            if (entity != null)
                current = value.PatchTo(Stamp(entity));
            else
                current = store.Add(Sign(value));
        }
    }

    public override TEntity this[object[] keys, params Expression<
        Func<TEntity, object>
    >[] expanders]
    {
        get
        {
            TEntity entity = this[keys];
            if (entity == null)
                return entity;
            if (expanders != null)
            {
                IQueryable<TEntity> query = entity.ToQueryable();
                if (expanders != null)
                {
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                    {
                        query = query.Include(expander);
                    }
                }
                entity = query.FirstOrDefault();
            }
            return entity;
        }
        set
        {
            TEntity entity = this[keys];
            if (entity != null)
            {
                IQueryable<TEntity> query = entity.ToQueryable();
                if (expanders != null)
                {
                    foreach (Expression<Func<TEntity, object>> expander in expanders)
                    {
                        query = query.Include(expander);
                    }
                }

                TEntity current = value.PatchTo(Stamp(entity));
            }
        }
    }

    public override object this[Expression<
        Func<TEntity, object>
    > selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders]
    {
        get
        {
            TEntity entity = this[keys];
            if (entity == null)
                return entity;
            IQueryable<TEntity> query = entity.ToQueryable();
            if (expanders != null)
            {
                foreach (Expression<Func<TEntity, object>> expander in expanders)
                {
                    query = query.Include(expander);
                }
            }
            return query.Select(selector).FirstOrDefault();
        }
        set
        {
            TEntity entity = this[keys];
            IQueryable<TEntity> query = entity.ToQueryable();
            if (expanders != null)
            {
                foreach (Expression<Func<TEntity, object>> expander in expanders)
                {
                    query = query.Include(expander);
                }
            }
            object s = query.Select(selector).FirstOrDefault();
            if (s != null)
            {
                value.PatchTo(s);
            }
        }
    }

    public override TEntity Add(TEntity entity)
    {
        return store.Add(Sign(entity));
    }

    public override TEntity Update(TEntity entity)
    {
        return store.Update(Stamp(entity));
    }

    public override IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entity)
    {
        return entity.ForEachAsync((e) => store.Add(Sign(e)));
    }

    public void AutoTransaction(bool enable)
    {
        store.Database.AutoTransactionsEnabled = enable;
    }

    public IDbContextTransaction BeginTransaction()
    {
        return store.Database.BeginTransaction();
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return store.Database.BeginTransactionAsync(Cancellation);
    }

    public void ChangeDetecting(bool enable = true)
    {
        if (InnerContext != null)
        {
            store.ChangeTracker.AutoDetectChangesEnabled = enable;
        }
    }

    public virtual async Task CommitTransaction(Task<IDbContextTransaction> transaction)
    {
        await (await transaction).CommitAsync(Cancellation);
    }

    public virtual void CommitTransaction(IDbContextTransaction transaction)
    {
        transaction.Commit();
    }

    public override TEntity Delete(TEntity entity)
    {
        return store.Remove(entity);
    }

    public void LazyLoading(bool enable)
    {
        store.ChangeTracker.LazyLoadingEnabled = enable;
    }

    public override TEntity NewEntry(params object[] parameters)
    {
        return store.Add(Sign(typeof(TEntity).New<TEntity>(parameters)));
    }

    public void QueryTracking(bool enable)
    {
        if (!enable)
            store.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        else
            store.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    public void TrackingEvents(bool enable = true)
    {
        if (InnerContext != null)
        {
            if (enable)
            {
                store.ChangeTracker.StateChanged += AuditStateEvent;
                store.ChangeTracker.StateChanged += LoadRemotesEvent;
                store.ChangeTracker.Tracked += LoadRemotesEvent;
            }
            else
            {
                store.ChangeTracker.StateChanged -= AuditStateEvent;
                store.ChangeTracker.StateChanged -= LoadRemotesEvent;
                store.ChangeTracker.Tracked -= LoadRemotesEvent;
            }
        }
    }

    public override object TracePatching(
        object item,
        string propertyName = null,
        Type type = null
    )
    {
        return store.AttachProperty(item, propertyName, type);
    }

    public override IQueryable<TEntity> AsQueryable()
    {
        return Query;
    }

    public override IQueryable<TEntity> Query => _query ??= store.EntitySet<TEntity>();

    protected override async Task<int> saveAsTransaction(
        CancellationToken token = default
    )
    {
        await using (
            IDbContextTransaction tr = await store.Database.BeginTransactionAsync(token)
        )
        {
            try
            {
                int changes = await store.Save(true, token);

                await tr.CommitAsync(token);

                return changes;
            }
            catch (DbUpdateException e)
            {
                if (e is DbUpdateConcurrencyException)
                    tr.Warning<Datalog>(
                        $"{$"Concurrency update exception data changed by: {e.Source}, "}{$"entries involved in detail data object"}",
                        e.Entries,
                        e
                    );
                tr.Failure<Datalog>(
                    $"{$"Fail on update database transaction Id:{tr.TransactionId}, using context:{store.GetType().Name},"}{$" TimeStamp:{DateTime.Now.ToString()}, changes made count"}"
                );

                await tr.RollbackAsync(token);

                tr.Warning<Datalog>($"Transaction Id:{tr.TransactionId} Rolling Back !!");
            }

            return -1;
        }
    }

    protected override async Task<int> saveChanges(
        CancellationToken token = default
    )
    {
        try
        {
            return await store.SaveChangesAsync(token);
        }
        catch (DbUpdateException e)
        {
            if (e is DbUpdateConcurrencyException)
                store.Warning<Datalog>(
                    $"{$"Concurrency update exception data changed by: {e.Source}, "}{$"entries involved in detail data object"}",
                    e.Entries,
                    e
                );
            store.Failure<Datalog>(
                $"{$"Fail on update database, using context:{store.GetType().Name}, "}{$"TimeStamp: {DateTime.Now.ToString()}"}"
            );
        }

        return -1;
    }
}

public class StoreRepository<TStore, TEntity>
    : StoreRepository<TEntity>,
        IStoreRepository<TStore, TEntity>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataStore
{
    public StoreRepository(
        IRepositoryContextPool<DataStoreContext<TStore>> pool,
        IEntityCache<TStore, TEntity> cache,
        IEnumerable<IRemoteProperty<TStore, TEntity>> remoteProps,
        IRemoteSynchronizer synchronizer
    ) : base(pool.ContextPool)
    {
        mapper = cache.Mapper;
        this.cache = cache;
        synchronizer.AddRepository(this);
        RemoteProperties = remoteProps.DoEach(
            (o) =>
            {
                o.Host = this;
                return o;
            }
        );
    }

    public override Task<int> Save(
        bool asTransaction,
        CancellationToken token = default
    )
    {
        return ContextLease.Save(asTransaction, token);
    }
}
