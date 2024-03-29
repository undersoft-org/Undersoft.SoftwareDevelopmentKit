﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Service.Data.Repository;

namespace Undersoft.SDK.Service.Data.Repository;

using Data.Entity;
using Invoking;
using Undersoft.SDK;
using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Remote;
using Undersoft.SDK.Service.Data.Remote.Repository;

public abstract class Repository : IRepository
{
    protected RelatedType relatedtype = RelatedType.None;
    protected Uscn serialcode;
    protected IDataMapper mapper;
    protected bool loaded;
    protected bool audited;

    private bool disposedValue;
    private Type contextType;

    protected Repository() { }

    protected Repository(object context)
    {
        InnerContext = context;
        PatchingEvent = new Invoker(this.TracePatching);
    }

    protected Repository(IRepositoryContext context)
    {
        Site = context.Site;
        context.Lease(this);
        PatchingEvent = new Invoker(this.TracePatching);
    }

    public Type ElementType { get; set; }

    public virtual string Name => ElementType.GetDataName();

    public virtual string FullName => ElementType.GetDataFullName();

    public object InnerContext { get; set; }

    public DataSite Site { get; set; }

    public Type ContextType => contextType ??= InnerContext.GetType();

    public Expression Expression { get; set; }

    public IQueryProvider Provider { get; set; }

    public IDataMapper Mapper => mapper ??= DataMapper.GetMapper();

    public Uscn Empty => Uscn.Empty;

    public CancellationToken Cancellation { get; set; } = new(false);

    public long Id
    {
        get => serialcode.Id == 0 ? serialcode.Id = Unique.NewId : serialcode.Id;
        set => serialcode.Id = value;
    }

    public long TypeId
    {
        get =>
            serialcode.TypeId == 0
                ? serialcode.TypeId = ContextType.UniqueKey32()
                : serialcode.TypeId;
        set => serialcode.TypeId = value;
    }

    public string CodeNo
    {
        get => serialcode.CodeNo;
        set => serialcode.CodeNo = value;
    }

    public IRepositoryContext ContextLease { get; set; }

    public IRepositoryContextPool ContextPool { get; set; }

    public bool Pooled => ContextPool != null;

    public bool Leased => ContextLease != null;

    public IEnumerable<IRemoteProperty> RemoteProperties { get; set; }

    public virtual int RemotesCount { get; set; }

    public virtual Towards Towards { get; set; }

    public byte[] GetBytes()
    {
        return serialcode.GetBytes();
    }

    public byte[] GetIdBytes()
    {
        return serialcode.GetIdBytes();
    }

    public bool AreEqually(IUnique other)
    {
        return serialcode.Equals(other);
    }

    public int CompareTo(IUnique other)
    {
        return serialcode.CompareTo(other);
    }

    protected virtual async void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                await Save(true);

                _ = ReleaseAsync().ConfigureAwait(false);

                ElementType = null;
                Expression = null;
                Provider = null;
                serialcode.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public virtual async ValueTask DisposeAsyncCore()
    {
        await new ValueTask(
            Task.Run(async () =>
            {
                await Save(true).ConfigureAwait(false);

                await ReleaseAsync().ConfigureAwait(false);

                ElementType = null;
                Expression = null;
                Provider = null;
                serialcode.Dispose();
            })
        ).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);

        GC.SuppressFinalize(this);
    }

    public virtual void ResetState()
    {
        if (Leased)
            ContextLease.ResetState();
    }

    public virtual Task ResetStateAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ResetState());
    }

    public virtual Task<int> Save(bool asTransaction, CancellationToken token = default)
    {
        if (Leased)
        {
            return ContextLease.Save(asTransaction, token);
        }
        else
        {
            switch (asTransaction)
            {
                case true:
                    return saveAsTransaction();
                default:
                    return saveChanges();
            }
        }
    }

    public virtual bool Release()
    {
        if (Leased)
            return ContextLease.Release();
        return false;
    }

    public virtual Task<bool> ReleaseAsync(CancellationToken token = default)
    {
        return ContextLease.ReleaseAsync();
    }

    public virtual void Lease(IRepositoryContext rentContext)
    {
        rentContext.Lease(this);
    }

    public virtual Task LeaseAsync(
        IRepositoryContext destContext,
        CancellationToken token = default
    )
    {
        return Task.Run(() => Lease(destContext), token);
    }

    public virtual void SnapshotConfiguration()
    {
        throw new NotImplementedException();
    }

    public virtual void LoadRemotes(object entity)
    {
        RemoteProperties.ForEach((o) => o.Load(entity));
    }

    public virtual async Task LoadRemotesAsync(object entity)
    {
        await Task.WhenAll(RemoteProperties.DoEach((o) => o.LoadAsync(entity)));
    }

    public virtual void LoadRelated(EntityEntry entry, RelatedType relatedType)
    {
        var modes = (int)(relatedType & (RelatedType.Reference | RelatedType.Collection));

        if (modes < 1)
            return;

        if (modes.IsOdd())
            entry.References.ForOnly(
                a => !a.IsLoaded,
                (e) =>
                {
                    e.Load();
                }
            );
        if (modes > 1)
            entry.Collections.ForOnly(
                a => !a.IsLoaded,
                (e) =>
                {
                    e.Load();
                }
            );
    }

    public virtual void LoadRelatedAsync(
        EntityEntry entry,
        RelatedType relatedType,
        CancellationToken token
    )
    {
        var modes = (int)(relatedType & (RelatedType.Reference | RelatedType.Collection));

        if (modes < 1)
            return;

        if (modes.IsOdd())
            entry.References
                .ForOnly(
                    a => !a.IsLoaded,
                    async (e) =>
                    {
                        await e.LoadAsync();
                    }
                )
                .Commit();
        if (modes > 1)
            entry.Collections
                .ForOnly(
                    a => !a.IsLoaded,
                    async (e) =>
                    {
                        await e.LoadAsync();
                    }
                )
                .Commit();
    }

    public virtual void LoadRemotesEvent(object sender, EntityEntryEventArgs e)
    {
        var entry = e.Entry;
        var entity = entry.Entity;
        var type = entity.GetDataType();

        if (type.IsAssignableTo(typeof(IEntity)) && type == ElementType)
        {
            RemoteProperties.DoEach(
                async (o) =>
                {
                    await o.LoadAsync(entity);
                }
            );
        }
    }

    protected virtual void AuditStateEvent(object sender, EntityEntryEventArgs e)
    {
        var entity = e.Entry.Entity as IOrigin;

        switch (e.Entry.State)
        {
            case EntityState.Deleted:
                entity.Stamp(entity);
                entity.Modified = DateTime.UtcNow;
                entity.SetFlag(StateFlags.Inactive, true);
                break;
            case EntityState.Modified:
                entity.Stamp(entity);
                entity.Modified = DateTime.UtcNow;
                break;
            case EntityState.Added:
                entity.Stamp(entity);
                entity.Created = DateTime.UtcNow;
                break;
        }

        audited = true;
    }

    public virtual object TracePatching(object item, string propertyName = null, Type type = null)
    {
        var dbContext = (DbContext)InnerContext;

        if (type == null)
            return dbContext.Attach(item).Entity;
        else if (type.IsAssignableTo(typeof(ICollection)))
        {
            var list = dbContext.Entry(item).Collection(propertyName);
            list.Load();
            return list.CurrentValue;
        }
        else
        {
            var obj = dbContext.Entry(item).Reference(propertyName);
            obj.Load();
            return obj.CurrentValue;
        }
    }

    public IInvoker PatchingEvent { get; set; }
    public string CodeNumber
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    protected abstract Task<int> saveAsTransaction(CancellationToken token = default);

    protected abstract Task<int> saveChanges(CancellationToken token = default);
}
