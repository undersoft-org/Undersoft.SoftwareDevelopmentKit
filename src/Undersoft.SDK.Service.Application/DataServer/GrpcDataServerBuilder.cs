using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;
using System;
using System.Linq;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.DataServer;

using Controller;
using Undersoft.SDK.Service.Application.Controller.Stream;
using Undersoft.SDK.Service.Data.Contract;

public class GrpcDataServerBuilder<TServiceStore> : DataServerBuilder, IDataServerBuilder<TServiceStore> where TServiceStore : IDataServiceStore
{
    static bool grpcadded = false;
    IServiceRegistry _registry;

    public GrpcDataServerBuilder() : base()
    {
        _registry = ServiceManager.GetManager().Registry;
        StoreType = typeof(TServiceStore);
    }

    public void AddControllers()
    {
        //Type[] storeTypes = DataStoreRegistry.Stores.Where(s => s.IsAssignableTo(StoreType)).ToArray();

        //if (!storeTypes.Any())
        //    return;

        Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
        var controllerTypes = asm.SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type => type.GetCustomAttribute<StreamDataServiceAttribute>()
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
                ifaceType = typeof(IStreamDataController<>).MakeGenericType(new[] { genTypes[4] });
            else if (genTypes.Length > 3)
                if (genTypes[3].IsAssignableTo(typeof(IContract)) && genTypes[1].IsAssignableTo(StoreType))
                    ifaceType = typeof(IStreamDataController<>).MakeGenericType(new[] { genTypes[3] });
                else
                    continue;

            GrpcDataServerRegistry.ServiceContracts.Add(ifaceType);

            _registry.AddSingleton(ifaceType, controllerType.New());
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
            return StoreRoutes.StreamEventStore;
        }
        else
        {
            return StoreRoutes.StreamDataStore;
        }
    }

    public virtual void AddGrpcServicer()
    {
        if (!grpcadded)
        {
            _registry.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System
                    .IO
                    .Compression
                    .CompressionLevel
                    .NoCompression;
            });
            _registry.AddSingleton(
                BinderConfiguration.Create(binder: new GrpcDataServerBinder(_registry))
            );
            _registry.AddCodeFirstGrpcReflection();
            grpcadded = true;
        }
    }
}