using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.DataServer;

using Controller;
using Undersoft.SDK.Service.Application.Controller.Crud;
using Undersoft.SDK.Service.Data.Object;

public class RestDataServerBuilder<TStore> : DataServerBuilder, IDataServerBuilder<TStore> where TStore : IDataServiceStore
{
    IServiceRegistry _registry;

    public RestDataServerBuilder() : base()
    {
        _registry = ServiceManager.GetManager().Registry;
        StoreType = typeof(TStore);
    }

    public void AddControllers()
    {

        Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
        var controllerTypes = asm.SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type => type.GetCustomAttribute<CrudDataServiceAttribute>()
                                    != null
                        )
                        .ToArray())
            .Where(
                b =>
                    !b.IsAbstract
                    && b.BaseType.IsGenericType
                    && b.BaseType.GenericTypeArguments.Length > 3
            ).ToArray();

        foreach (var controllerType in controllerTypes)
        {
            Type ifaceType = null;
            var genTypes = controllerType.BaseType.GenericTypeArguments;

            if (genTypes.Length > 4 && genTypes[1].IsAssignableTo(StoreType) && genTypes[2].IsAssignableTo(StoreType))
                ifaceType = typeof(ICrudDataController<,,>).MakeGenericType(new[] { genTypes[0], genTypes[3], genTypes[4] });
            else if (genTypes.Length > 3)
                if (genTypes[3].IsAssignableTo(typeof(IDataObject)) && genTypes[1].IsAssignableTo(StoreType))
                    ifaceType = typeof(ICrudDataController<,,>).MakeGenericType(new[] { genTypes[0], genTypes[2], genTypes[3] });
                else
                    continue;
        }
    }

    public override void Build()
    {
        AddControllers();
    }

    protected override string GetRoutes()
    {
        if (StoreType == typeof(IEventStore))
        {
            return StoreRoutes.CrudEventStore;
        }
        else
        {
            return StoreRoutes.CrudDataStore;
        }
    }
}