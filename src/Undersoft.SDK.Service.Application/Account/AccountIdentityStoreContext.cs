using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Service.Application.Account;

public partial class IdentityStore<TStore, TContext> : AccountIdentityStoreContext<TStore> where TStore : IDatabaseStore where TContext : DbContext
{
    public IdentityStore(DbContextOptions<TContext> options) : base(options)
    {
    }
}

public partial class AccountIdentityStoreContext<TStore> : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long>, IDataStoreContext<TStore> where TStore : IDatabaseStore
{
    public AccountIdentityStoreContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("AccountIdentity");
        builder.Entity<IdentityUser<long>>(entity =>
        {
            entity.ToTable(name: "Users");
        });
        builder.Entity<IdentityRole<long>>(entity =>
        {
            entity.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<long>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        builder.Entity<IdentityUserClaim<long>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<long>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        builder.Entity<IdentityRoleClaim<long>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<long>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
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
