using Microsoft.OData.Client;
using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Service.Data.Store;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Repository;

using Entity;
using Instant.Rubrics;
using Undersoft.SDK.Service.Data.Repository.Client.Remote;
using Undersoft.SDK.Service.Data.Service.Remote;

public class RepositoryLink<TStore, TOrigin, TTarget> : RemoteRepository<TStore, TTarget>, IRepositoryLink<TStore, TOrigin, TTarget>
    where TOrigin : Entity
    where TTarget : Entity
    where TStore : IDataServiceStore
{
    IServiceRemote<TOrigin, TTarget> relation;

    public RepositoryLink(
        IRepositoryContextPool<OpenDataService<TStore>> pool,
        IEntityCache<TStore, TTarget> cache,
        IServiceRemote<TOrigin, TTarget> relation,
        IRemoteSynchronizer synchronizer) : base(pool, cache)
    {
        this.relation = relation;
        Synchronizer = synchronizer;
    }

    public Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    { return relation.CreatePredicate(entity); }

    public void Load(object origin) { Load(origin, dsContext); }

    public void Load<T>(IEnumerable<T> origins, OpenDataService context) where T : class
    { origins.DoEach((o) => Load(o, context)); }

    public void Load(object origin, OpenDataService context)
    {
        IInnerProxy _entity = (IInnerProxy)origin;
        int rubricId = RemoteMember.RubricId;

        Expression<Func<TTarget, bool>> predicate = CreatePredicate(origin);
        if (predicate != null)
        {
            IRemoteSet<TTarget> remote;
            switch (Towards)
            {
                case Towards.ToSingle:
                    DataServiceQuery<TTarget> query = context.CreateQuery<TTarget>(typeof(TTarget).Name);
                    Synchronizer.AcquireLinker();
                    _entity[rubricId] = query.FirstOrDefault(predicate);
                    Synchronizer.ReleaseLinker();
                    break;
                case Towards.ToSet:
                    remote = new RemoteSet<TTarget>(context);
                    remote.LoadCompleted += Synchronizer.OnLinked;
                    _entity[rubricId] = remote;
                    remote.LoadAsync(predicate);
                    Synchronizer.AcquireLinker();
                    break;
                case Towards.SetToSet:
                    remote = new RemoteSet<TTarget>(context);
                    remote.LoadCompleted += Synchronizer.OnLinked;
                    _entity[rubricId] = remote;
                    remote.LoadAsync(predicate);
                    Synchronizer.AcquireLinker();
                    break;
                default:
                    break;
            }
        }
    }

    public async Task LoadAsync(object origin) { await Task.Run(() => Load(origin, dsContext), Cancellation); }

    public async ValueTask LoadAsync(object origin, OpenDataService context, CancellationToken token)
    { await Task.Run(() => Load(origin, context), token); }

    public override Task<int> Save(bool asTransaction, CancellationToken token = default)
    { return ContextLease.Save(asTransaction, token); }

    public IRepository Host { get; set; }

    public bool IsLinked { get; set; }

    public override int LinkedCount { get; set; }

    public MemberRubric RemoteMember => relation.RemoteMember;

    public Expression<Func<TOrigin, object>> OriginKey
    {
        get => relation.OriginKey;
        set => relation.OriginKey = value;
    }

    public Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate
    {
        get => relation.Predicate;
        set => relation.Predicate = value;
    }

    public IRemoteSynchronizer Synchronizer { get; }

    public Expression<Func<TTarget, object>> TargetKey
    {
        get => relation.TargetKey;
        set => relation.TargetKey = value;
    }

    public override Towards Towards => relation.Towards;
}
