using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Undersoft.SDK.Logging;

namespace Undersoft.SDK.Service.Data.Store;

using Uniques;

public class DataStoreContext<TStore> : DataStoreContext, IDataStoreContext<TStore>
    where TStore : IDatabaseStore
{
    protected virtual Type StoreType { get; }

    public DataStoreContext(DbContextOptions options, IServicer servicer = null)
        : base(options, servicer)
    {
        StoreType = typeof(TStore);
    }
}

public class DataStoreContext : DbContext, IDataStoreContext, IResettableService
{
    public virtual IServicer servicer { get; }

    public override IModel Model
    {
        get { return base.Model; }
    }

    public DataStoreContext(DbContextOptions options, IServicer servicer = null) : base(options)
    {
        this.servicer = servicer;
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IUniqueIdentifiable
    {
        return Set<TEntity>();
    }

    public object EntitySet<TEntity>() where TEntity : class, IUniqueIdentifiable
    {
        return Set<TEntity>();
    }

    public object EntitySet(Type type)
    {
        return this.GetEntitySet(type);
    }

    public virtual Task<int> Save(bool asTransaction, CancellationToken token = default)
    {
        return saveEndpoint(asTransaction, token);
    }

    private async Task<int> saveEndpoint(bool asTransaction, CancellationToken token = default)
    {
        if (ChangeTracker.HasChanges())
        {
            if (asTransaction)
                return await saveAsTransaction(token);
            else
                return await saveChanges(token);
        }
        return 0;
    }

    private async Task<int> saveAsTransaction(CancellationToken token = default)
    {
        await using var tr = await Database.BeginTransactionAsync(token);
        try
        {
            var changes = await SaveChangesAsync(token);

            await tr.CommitAsync(token);

            return changes;
        }
        catch (DbUpdateException e)
        {
            if (e is DbUpdateConcurrencyException)
                tr.Warning<Datalog>(
                    $"Concurrency update exception data changed by: {e.Source}, "
                        + $"entries involved in detail data object",
                    e.Entries,
                    e
                );
            else
                tr.Failure<Datalog>(
                    $"Fail on update database transaction Id:{tr.TransactionId}, using context:{GetType().Name},"
                        + $" context Id:{ContextId}, TimeStamp:{DateTime.Now.ToString()} {e.StackTrace}",
                    e.Entries
                );

            await tr.RollbackAsync(token);

            tr.Warning<Datalog>($"Transaction Id:{tr.TransactionId} Rolling Back !!");
        }
        return -1;
    }

    private async Task<int> saveChanges(CancellationToken token = default)
    {
        try
        {
            return await SaveChangesAsync(token);
        }
        catch (DbUpdateException e)
        {
            if (e is DbUpdateConcurrencyException)
                this.Warning<Datalog>(
                    $"Concurrency update exception data changed by: {e.Source}, "
                        + $"entries involved in detail data object",
                    e.Entries,
                    e
                );
            else
                this.Failure<Datalog>(
                    $"Fail on update database, using context:{GetType().Name}, "
                        + $"context Id: {ContextId}, TimeStamp: {DateTime.Now.ToString()}"
                );
        }

        return -1;
    }
}
