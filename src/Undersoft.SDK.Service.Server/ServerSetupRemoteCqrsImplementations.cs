using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Server;

using Operation.Remote.Command;
using Operation.Remote.Command.Handler;
using Operation.Remote.Command.Validator;
using Operation.Remote.Query;
using Operation.Remote.Query.Handler;
using Operation.Remote.Validator;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Server.Operation.Remote.Command.Notification;
using Undersoft.SDK.Service.Server.Operation.Remote.Command.Notification.Handler;

public partial class ServerSetup
{
    public IServerSetup AddServerSetupRemoteCqrsImplementations()
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
                                                .IsAssignableTo(typeof(DataRemoteAttribute))
                                    )
                        )
                        .ToArray()
            )
            .Where(
                b =>
                    !b.IsAbstract
                    && b.BaseType.IsGenericType
                    && b.BaseType.GenericTypeArguments.Length > 3
            )
            .ToArray();

        foreach (var controllerType in controllerTypes)
        {
            var genericTypes = controllerType.BaseType.GenericTypeArguments;
            var store = genericTypes[1];
            var model = genericTypes[2];
            var dto = genericTypes[3];

            service.AddTransient(
                typeof(IRequest<>).MakeGenericType(
                    typeof(RemoteCommand<>).MakeGenericType(model)
                ),
                typeof(RemoteCommand<>).MakeGenericType(model)
            );

            service.AddTransient(
                typeof(RemoteCommandValidatorBase<>).MakeGenericType(
                    typeof(RemoteCommand<>).MakeGenericType(model)
                ),
                typeof(RemoteCommandValidator<>).MakeGenericType(model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteFilter<,,>).MakeGenericType(store, dto, model),
                        typeof(ISeries<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteFilterHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteFind<,,>).MakeGenericType(store, dto, model),
                    model
                ),
                typeof(RemoteFindHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteFindQuery<,,>).MakeGenericType(store, dto, model),
                    typeof(IQueryable<>).MakeGenericType(model)
                ),
                typeof(RemoteFindQueryHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteGet<,,>).MakeGenericType(store, dto, model),
                    typeof(ISeries<>).MakeGenericType(model)
                ),
                typeof(RemoteGetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteGetQuery<,,>).MakeGenericType(store, dto, model),
                    typeof(IQueryable<>).MakeGenericType(model)
                ),
                typeof(RemoteGetQueryHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteCreate<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommand<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteCreateHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteUpdate<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommand<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteUpdateHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteChange<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommand<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteChangeHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteDelete<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommand<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteDeleteHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteChangeSet<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommandSet<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteChangeSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteUpdateSet<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommandSet<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteUpdateSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteCreateSet<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommandSet<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteCreateSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteDeleteSet<,,>).MakeGenericType(store, dto, model),
                        typeof(RemoteCommandSet<>).MakeGenericType(model)
                    }
                ),
                typeof(RemoteDeleteSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteDeletedSet<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteDeletedSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteUpdatedSet<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteUpdatedSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteCreatedSet<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteCreatedSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteChangedSet<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteChangedSetHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteChanged<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteChangedHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteCreated<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteCreatedHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteDeleted<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteDeletedHandler<,,>).MakeGenericType(store, dto, model)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteUpdated<,,>).MakeGenericType(store, dto, model)
                ),
                typeof(RemoteUpdatedHandler<,,>).MakeGenericType(store, dto, model)
            );
        }
        return this;
    }
}
