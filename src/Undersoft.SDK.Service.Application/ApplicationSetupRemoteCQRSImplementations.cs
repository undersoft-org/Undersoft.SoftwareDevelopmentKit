using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Application;

using Operation.Remote.Command;
using Operation.Remote.Command.Handler;
using Operation.Remote.Command.Validator;
using Operation.Remote.Notification;
using Operation.Remote.Notification.Handler;
using Operation.Remote.Query;
using Operation.Remote.Query.Handler;
using Operation.Remote.Validator;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Application.Operation.Command;

public partial class ApplicationSetup
{
    public IApplicationSetup AddApplicationSetupRemoteCQRSImplementations(Assembly[] assemblies)
    {
        IServiceRegistry service = registry;

        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

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
                                                .IsAssignableTo(typeof(RemoteDataServiceAttribute))
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
            var _viewmodel = genericTypes[3];
            var dtoType = genericTypes[2];

            service.AddTransient(
                typeof(IRequest<>).MakeGenericType(
                    typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                ),
                typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
            );

            service.AddTransient(
                typeof(RemoteCommandValidatorBase<>).MakeGenericType(
                    typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                ),
                typeof(RemoteCommandValidator<>).MakeGenericType(_viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteFilter<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(ISeries<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteFilterHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteFind<,,>).MakeGenericType(store, dtoType, _viewmodel),
                    _viewmodel
                ),
                typeof(RemoteFindHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteFindQuery<,,>).MakeGenericType(store, dtoType, _viewmodel),
                    typeof(IQueryable<>).MakeGenericType(_viewmodel)
                ),
                typeof(RemoteFindQueryHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteGet<,,>).MakeGenericType(store, dtoType, _viewmodel),
                    typeof(ISeries<>).MakeGenericType(_viewmodel)
                ),
                typeof(RemoteGetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(RemoteGetQuery<,,>).MakeGenericType(store, dtoType, _viewmodel),
                    typeof(IQueryable<>).MakeGenericType(_viewmodel)
                ),
                typeof(RemoteGetQueryHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteCreate<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteCreateHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteUpdate<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteUpdateHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteChange<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteChangeHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteDelete<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommand<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteDeleteHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteExecute<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(ActionCommand<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteExecuteHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteChangeSet<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteChangeSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteUpdateSet<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteUpdateSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteCreateSet<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteCreateSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    new[]
                    {
                        typeof(RemoteDeleteSet<,,>).MakeGenericType(store, dtoType, _viewmodel),
                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                    }
                ),
                typeof(RemoteDeleteSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteExecuted<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteExecutedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteDeletedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteDeletedSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteUpdatedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteUpdatedSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteCreatedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteCreatedSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddScoped(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteChangedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteChangedSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteChanged<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteChangedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteCreated<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteCreatedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteDeleted<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteDeletedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );

            service.AddTransient(
                typeof(INotificationHandler<>).MakeGenericType(
                    typeof(RemoteUpdated<,,>).MakeGenericType(store, dtoType, _viewmodel)
                ),
                typeof(RemoteUpdatedHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
            );
        }
        return this;
    }
}
