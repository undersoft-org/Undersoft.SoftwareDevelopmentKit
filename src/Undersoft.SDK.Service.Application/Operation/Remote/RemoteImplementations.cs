using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Operation.Remote;

using Behaviour;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OData.Client;
using Operation.Remote.Command;
using Operation.Remote.Command.Handler;
using Operation.Remote.Command.Validator;
using Operation.Remote.Notification;
using Operation.Remote.Notification.Handler;
using Operation.Remote.Query;
using Operation.Remote.Query.Handler;
using Operation.Remote.Validator;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Application.ViewModel;
using Undersoft.SDK.Service.Data.Branch;

public static class RemoteImplementations
{
    public static void AddOpenDataImplementations(this OpenDataService context, Assembly[] assemblies)
    {
        IServiceRegistry service = ServiceManager.GetRegistry();

        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        Type[] viewmodels = assemblies
            .SelectMany(
                a => a.DefinedTypes.Where(t => t.UnderlyingSystemType.IsAssignableTo(typeof(IViewModel)))
            )
            .Select(t => t.UnderlyingSystemType)
            .ToArray();

        IServiceCollection deck = service
            .AddTransient<ISeries<IViewModel>, Registry<IViewModel>>()
            .AddScoped<ITypedSeries<IViewModel>, TypedRegistry<IViewModel>>();

        Catalog<string> duplicateCheck = new Catalog<string>();
        Type[] stores = new Type[] { typeof(IDataStore) };
      
        foreach (var dtoName in context.GetEdmEntityTypes().Select(n => n.Name))
        {
            if (duplicateCheck.TryAdd(dtoName))
            {
                var dtoType = OpenDataServiceRegistry.GetMappedType(dtoName);
                if (dtoType == null)
                    continue;

                foreach (Type viewmodel in viewmodels)
                {
                    Type _viewmodel = viewmodel;
                    if (!_viewmodel.Name.Contains($"{dtoType.Name}"))
                    {
                        if (dtoType.IsGenericType
                        && dtoType.IsAssignableTo(typeof(Identifier))
                        && _viewmodel.Name.Contains(dtoType.GetGenericArguments().FirstOrDefault().Name))
                        {
                            _viewmodel = typeof(Identifier<>).MakeGenericType(_viewmodel);
                        }
                        else if (dtoType == typeof(EventBranch) && _viewmodel.Name.Contains("Event"))
                        {
                            _viewmodel = typeof(Event);
                        }
                        else
                            continue;
                    }

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
                    foreach (Type store in stores)
                    {
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
                                        typeof(RemoteChangeSet<,,>).MakeGenericType(
                                            store,
                                            dtoType,
                                            _viewmodel
                                        ),
                                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                                }
                            ),
                            typeof(RemoteChangeSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                        );

                        service.AddScoped(
                            typeof(IRequestHandler<,>).MakeGenericType(
                                new[]
                                {
                                        typeof(RemoteUpdateSet<,,>).MakeGenericType(
                                            store,
                                            dtoType,
                                            _viewmodel
                                        ),
                                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                                }
                            ),
                            typeof(RemoteUpdateSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                        );

                        service.AddScoped(
                            typeof(IRequestHandler<,>).MakeGenericType(
                                new[]
                                {
                                        typeof(RemoteCreateSet<,,>).MakeGenericType(
                                            store,
                                            dtoType,
                                            _viewmodel
                                        ),
                                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                                }
                            ),
                            typeof(RemoteCreateSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                        );

                        service.AddScoped(
                            typeof(IRequestHandler<,>).MakeGenericType(
                                new[]
                                {
                                        typeof(RemoteDeleteSet<,,>).MakeGenericType(
                                            store,
                                            dtoType,
                                            _viewmodel
                                        ),
                                        typeof(RemoteCommandSet<>).MakeGenericType(_viewmodel)
                                }
                            ),
                            typeof(RemoteDeleteSetHandler<,,>).MakeGenericType(store, dtoType, _viewmodel)
                        );
                        service.AddScoped(
                            typeof(INotificationHandler<>).MakeGenericType(
                                typeof(RemoteDeletedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                            ),
                            typeof(RemoteDeletedSetHandler<,,>).MakeGenericType(
                                store,
                                dtoType,
                                _viewmodel
                            )
                        );

                        service.AddScoped(
                            typeof(INotificationHandler<>).MakeGenericType(
                                typeof(RemoteUpdatedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                            ),
                            typeof(RemoteUpdatedSetHandler<,,>).MakeGenericType(
                                store,
                                dtoType,
                                _viewmodel
                            )
                        );

                        service.AddScoped(
                            typeof(INotificationHandler<>).MakeGenericType(
                                typeof(RemoteCreatedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                            ),
                            typeof(RemoteCreatedSetHandler<,,>).MakeGenericType(
                                store,
                                dtoType,
                                _viewmodel
                            )
                        );

                        service.AddScoped(
                            typeof(INotificationHandler<>).MakeGenericType(
                                typeof(RemoteChangedSet<,,>).MakeGenericType(store, dtoType, _viewmodel)
                            ),
                            typeof(RemoteChangedSetHandler<,,>).MakeGenericType(
                                store,
                                dtoType,
                                _viewmodel
                            )
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
                }
            }

        }
    }
}
