using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Server;

using Data.Model;
using Series;
using System.Collections.Generic;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Server.Operation.Invocation;
using Undersoft.SDK.Service.Server.Operation.Invocation.Handler;
using Undersoft.SDK.Service.Server.Operation.Invocation.Notification;
using Undersoft.SDK.Service.Server.Operation.Invocation.Notification.Handler;

public partial class ServerSetup
{
    public IServerSetup AddServerSetupInvocationImplementations()
    {
        IServiceRegistry service = registry;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        IServiceCollection deck = service
            .AddTransient<ISeries<IModel>, Registry<IModel>>()
            .AddScoped<ITypedSeries<IModel>, TypedRegistry<IModel>>();

        var controllerTypes = assemblies
            .SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type =>
                                type.GetCustomAttributes()
                                    .Any(a => a.GetType().IsAssignableTo(typeof(ServiceClientAttribute)))
                        )
                        .ToArray()
            )
            .Where(
                b =>
                    !b.IsAbstract
                    && b.BaseType.IsGenericType
                    && b.BaseType.GenericTypeArguments.Length > 2
            )
            .ToArray();

        HashSet<string> duplicateCheck = new HashSet<string>();

        foreach (var controllerType in controllerTypes)
        {
            var genericTypes = controllerType.BaseType.GenericTypeArguments;
            var store = genericTypes[1];
            var actionType = genericTypes[4];
            var dtoType = genericTypes[3];

            if (duplicateCheck.Add(store.Name + actionType.Name + dtoType.Name))
            {
                service.AddTransient(
                    typeof(IRequest<>).MakeGenericType(
                        typeof(Invocation<>).MakeGenericType(dtoType)
                    ),
                    typeof(Invocation<>).MakeGenericType(dtoType)
                );
                service.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        new[]
                        {
                            typeof(Action<,,>).MakeGenericType(store, actionType, dtoType),
                            typeof(Invocation<>).MakeGenericType(dtoType)
                        }
                    ),
                    typeof(ActionHandler<,,>).MakeGenericType(store, actionType, dtoType)
                );
                service.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        new[]
                        {
                            typeof(Setup<,,>).MakeGenericType(store, actionType, dtoType),
                            typeof(Invocation<>).MakeGenericType(dtoType)
                        }
                    ),
                    typeof(SetupHandler<,,>).MakeGenericType(store, actionType, dtoType)
                );
                service.AddTransient(
                    typeof(INotificationHandler<>).MakeGenericType(
                        typeof(ActionInvoked<,,>).MakeGenericType(store, actionType, dtoType)
                    ),
                    typeof(ActionInvokedHandler<,,>).MakeGenericType(store, actionType, dtoType)
                );
                service.AddTransient(
                    typeof(INotificationHandler<>).MakeGenericType(
                        typeof(SetupInvoked<,,>).MakeGenericType(store, actionType, dtoType)
                    ),
                    typeof(SetupInvokedHandler<,,>).MakeGenericType(store, actionType, dtoType)
                );
            }
        }
        return this;
    }
}
