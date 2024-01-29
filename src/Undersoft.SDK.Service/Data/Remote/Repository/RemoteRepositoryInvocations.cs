using Microsoft.OData.Client;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Remote.Repository;

public partial class RemoteRepository<TEntity>
{
    public async Task<TEntity> Access<TService>(
       Expression<Func<TService, Delegate>> method, string key,
       Arguments arguments
   )
    {
        return await Access(LinqExtension.GetMemberName(method), key, arguments);
    }

    public async Task<TEntity> Action<TService>(
        Expression<Func<TService, Delegate>> method, string key,
        Arguments arguments
    )
    {
        return await Action(LinqExtension.GetMemberName(method), key, arguments);
    }

    public async Task<TEntity> Setup<TService>(
        Expression<Func<TService, Delegate>> method, string key,
        Arguments arguments
    )
    {
        return await Setup(LinqExtension.GetMemberName(method), key, arguments);
    }

    public async Task<IEnumerable<TEntity>> Access<TService>(
        Expression<Func<TService, Delegate>> method,
        Arguments arguments
    )
    {
        return await Access(LinqExtension.GetMemberName(method), arguments);
    }

    public async Task<IEnumerable<TEntity>> Action<TService>(
        Expression<Func<TService, Delegate>> method,
        Arguments arguments
    )
    {
        return await Action(LinqExtension.GetMemberName(method), arguments);
    }

    public async Task<IEnumerable<TEntity>> Setup<TService>(
        Expression<Func<TService, Delegate>> method,
        Arguments arguments
    )
    {
        return await Setup(LinqExtension.GetMemberName(method), arguments);
    }

    public async Task<TEntity> Setup(string name, string key, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Setup", key,
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    public async Task<TEntity> Access(string name, string key, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Access", key,
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    public async Task<TEntity> Action(string name, string key, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Action", key,
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    public async Task<IEnumerable<TEntity>> Setup(string name, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Setup",
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    public async Task<IEnumerable<TEntity>> Access(string name, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Access",
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    public async Task<IEnumerable<TEntity>> Action(string name, Arguments arguments)
    {
        arguments.New("Name", name);
        return await InvokeAsync(
            "Action",
            arguments.ForEach(p => new BodyOperationParameter(p.Name, p.Value)).Commit()
        );
    }

    private async Task<TEntity> InvokeAsync(
        string invokeType,
        string key,
        BodyOperationParameter[] parameters
    )
    {
        var action = new DataServiceActionQuerySingle<TEntity>(
            remoteContext,
            $"{remoteContext.BaseUri.OriginalString}/{Name}({key})/{invokeType}",
            parameters
        );
        var result = await action.GetValueAsync();
        return result;
    }

    private async Task<IEnumerable<TEntity>> InvokeAsync(
        string invokeType,
        BodyOperationParameter[] parameters
    )
    {
        var action = new DataServiceActionQuery<TEntity>(
            remoteContext,
            $"{remoteContext.BaseUri.OriginalString}/{Name}/{invokeType}",
            parameters
        );
        var result = await action.ExecuteAsync();
        return result;
    }
}
