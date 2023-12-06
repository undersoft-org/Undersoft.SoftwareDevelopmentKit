using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository.Source
{
    public interface IRepositorySource<TStore, TEntity> : IRepositorySource where TEntity : class, IUniqueIdentifiable
    {
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        DbSet<TEntity> EntitySet();
    }

    public interface IRepositorySource : IRepositoryContextPool
    {
        IDataStoreContext CreateContext(DbContextOptions options);

        IDataStoreContext CreateContext(Type contextType, DbContextOptions options);

        object EntitySet<TEntity>() where TEntity : class, IUniqueIdentifiable;

        object EntitySet(Type entityType);

        IDataStoreContext Context { get; }

        DbContextOptions Options { get; }
    }

    public interface IRepositorySource<TContext> : IRepositoryContextPool<TContext>, IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext>, IRepositorySource
        where TContext : DbContext
    {
        TContext CreateContext(DbContextOptions<TContext> options);

        new DbContextOptions<TContext> Options { get; }
    }
}
