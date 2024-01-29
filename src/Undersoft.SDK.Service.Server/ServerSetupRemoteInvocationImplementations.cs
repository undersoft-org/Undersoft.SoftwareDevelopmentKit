using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Server;

using Castle.Core.Internal;
using Microsoft.IdentityModel.Tokens;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Server.Operation.Invocation;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation.Handler;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation.Notification;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation.Notification.Handler;

public partial class ServerSetup
{
    public IServerSetup AddServerSetupRemoteInvocationImplementations()
    {
        IServiceRegistry service = registry;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var controllerTypes = assemblies
            .SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type =>
                                type.GetCustomAttributes()
                                    .Any(
                                        a =>
                                            a.GetType()
                                                .IsAssignableTo(
                                                    typeof(ServiceRemoteAttribute)
                                                )
                                    )
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
            Type store = null, _viewmodel = null, dtoType = null;
            var genericTypes = controllerType.BaseType.GenericTypeArguments;
            if (genericTypes.Length > 3)
            {
                if (genericTypes.Length > 5)
                {
                    store = genericTypes[1];
                    _viewmodel = genericTypes[3];
                    dtoType = genericTypes[4];
                }
                else
                {
                    store = genericTypes[1];
                    _viewmodel = genericTypes[2];
                    dtoType = genericTypes[3];
                }
            }
            else
                continue;
            
            var concatNames = store.FullName + _viewmodel.FullName + dtoType.FullName;
            if (!concatNames.IsNullOrEmpty() && duplicateCheck.Add(concatNames))
            {
                service.AddTransient(
                    typeof(IRequest<>).MakeGenericType(
                        typeof(Invocation<>).MakeGenericType(_viewmodel)
                    ),
                    typeof(Invocation<>).MakeGenericType(_viewmodel)
                );

                service.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        new[]
                        {
                            typeof(RemoteAction<,,>).MakeGenericType(store, dtoType, _viewmodel),
                            typeof(Invocation<>).MakeGenericType(_viewmodel)
                        }
                    ),
                    typeof(RemoteActionHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                );
                service.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(
                        new[]
                        {
                            typeof(RemoteSetup<,,>).MakeGenericType(store, dtoType, _viewmodel),
                            typeof(Invocation<>).MakeGenericType(_viewmodel)
                        }
                    ),
                    typeof(RemoteSetupHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                );
                service.AddTransient(
                    typeof(INotificationHandler<>).MakeGenericType(
                        typeof(RemoteActionInvoked<,,>).MakeGenericType(store, dtoType, _viewmodel)
                    ),
                    typeof(RemoteActionInvokedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                );
                service.AddTransient(
                  typeof(INotificationHandler<>).MakeGenericType(
                      typeof(RemoteSetupInvoked<,,>).MakeGenericType(store, dtoType, _viewmodel)
                  ),
                  typeof(RemoteSetupInvokedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
              );
            }
        }
        return this;
    }
}
