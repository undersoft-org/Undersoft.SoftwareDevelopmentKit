using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Server;

using Behaviour;
using Data.Identifier;
using Operation.Command;
using Operation.Command.Handler;
using Operation.Command.Validator;
using Operation.Query;
using Operation.Query.Handler;
using Undersoft.SDK.Service.Data.Contract;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Command.Notification;
using Undersoft.SDK.Service.Server.Operation.Command.Notification.Handler;

public partial class ServerSetup
{
    public IServerSetup AddServerSetupCqrsImplementations()
    {
        IServiceRegistry service = registry;

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        service.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Singleton, null, true);

        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        service.AddMediatR(assemblies);

        Type[] dtos = assemblies
            .SelectMany(
                a =>
                    a.DefinedTypes.Where(
                        t =>
                            t.UnderlyingSystemType.IsAssignableTo(typeof(IContract))
                            || (
                                t.UnderlyingSystemType.IsGenericType
                                && t.UnderlyingSystemType.GetGenericArguments()[0].IsAssignableTo(
                                    typeof(IContract)
                                )
                            )
                    )
            )
            .Select(t => t.UnderlyingSystemType)
            .ToArray();

        IDataMapper mapper = registry.GetObject<IDataMapper>();

        IServiceCollection deck = service
            .AddTransient<ISeries<IEntity>, Listing<IEntity>>()
            .AddTransient<ISeries<IContract>, Chain<IContract>>()
            .AddTransient<ISeries<IEntity>, Registry<IEntity>>()
            .AddTransient<ISeries<IContract>, Catalog<IContract>>()
            .AddScoped<ITypedSeries<IEntity>, TypedRegistry<IEntity>>()
            .AddScoped<ITypedSeries<IContract>, TypedCatalog<IContract>>();

        Catalog<Type> duplicateCheck = new Catalog<Type>();
        Type[] stores = DataStoreRegistry.Stores
            .Where(s => s.IsAssignableTo(typeof(IDataServiceStore)))
            .ToArray();

        foreach (ISeries<IEntityType> contextEntityTypes in DataStoreRegistry.EntityTypes)
        {
            foreach (IEntityType _entityType in contextEntityTypes)
            {
                Type entityType = _entityType.ClrType;
                if (duplicateCheck.TryAdd(entityType))
                {
                    foreach (Type _dto in dtos)
                    {
                        Type dto = _dto;
                        if (!dto.Name.Contains($"{entityType.Name}"))
                        {
                            if (
                                entityType.IsGenericType
                                && entityType.IsAssignableTo(typeof(Identifier))
                                && dto.Name.Contains(
                                    entityType.GetGenericArguments().FirstOrDefault().Name
                                )
                            )
                            {
                                dto = typeof(Identifier<>).MakeGenericType(_dto);
                            }
                            else
                                continue;
                        }
                        service.AddTransient(
                            typeof(IRequest<>).MakeGenericType(
                                typeof(Command<>).MakeGenericType(dto)
                            ),
                            typeof(Command<>).MakeGenericType(dto)
                        );

                        service.AddTransient(
                            typeof(CommandValidatorBase<>).MakeGenericType(
                                typeof(Command<>).MakeGenericType(dto)
                            ),
                            typeof(CommandValidator<>).MakeGenericType(dto)
                        );

                        foreach (Type store in stores)
                        {
                            service.AddTransient(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(FilterAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        dto
                                    }
                                ),
                                typeof(FilterAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Filter<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(ISeries<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(FilterHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(GetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        dto
                                    }
                                ),
                                typeof(GetAsyncHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    typeof(Find<,,>).MakeGenericType(store, entityType, dto),
                                    dto
                                ),
                                typeof(FindHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    typeof(FindQuery<,,>).MakeGenericType(store, entityType, dto),
                                    typeof(IQueryable<>).MakeGenericType(dto)
                                ),
                                typeof(FindQueryHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    typeof(Get<,,>).MakeGenericType(store, entityType, dto),
                                    typeof(ISeries<>).MakeGenericType(dto)
                                ),
                                typeof(GetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    typeof(GetQuery<,,>).MakeGenericType(store, entityType, dto),
                                    typeof(IQueryable<>).MakeGenericType(dto)
                                ),
                                typeof(GetQueryHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Create<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(CreateHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Upsert<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpsertHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Update<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpdateHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Change<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(ChangeHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(Delete<,,>).MakeGenericType(store, entityType, dto),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(DeleteHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(ChangeSetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(ChangeSetAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(ChangeSet<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(CommandSet<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(ChangeSetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(UpdateSet<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(CommandSet<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpdateSetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(UpdateSetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpdateSetAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(CreateSet<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(CommandSet<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(CreateSetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(CreateSetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(CreateSetAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(UpsertSet<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(CommandSet<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpsertSetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(UpsertSetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(UpsertSetAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(IRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(DeleteSet<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(CommandSet<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(DeleteSetHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddScoped(
                                typeof(IStreamRequestHandler<,>).MakeGenericType(
                                    new[]
                                    {
                                        typeof(DeleteSetAsync<,,>).MakeGenericType(
                                            store,
                                            entityType,
                                            dto
                                        ),
                                        typeof(Command<>).MakeGenericType(dto)
                                    }
                                ),
                                typeof(DeleteSetAsyncHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(DeletedSet<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(DeletedSetHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(UpsertedSet<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(UpsertedSetHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(UpdatedSet<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(UpdatedSetHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(CreatedSet<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(CreatedSetHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddScoped(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(ChangedSet<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(ChangedSetHandler<,,>).MakeGenericType(
                                    store,
                                    entityType,
                                    dto
                                )
                            );
                            service.AddTransient(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(Changed<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(ChangedHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(Created<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(CreatedHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(Deleted<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(DeletedHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(Upserted<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(UpsertedHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                            service.AddTransient(
                                typeof(INotificationHandler<>).MakeGenericType(
                                    typeof(Updated<,,>).MakeGenericType(store, entityType, dto)
                                ),
                                typeof(UpdatedHandler<,,>).MakeGenericType(store, entityType, dto)
                            );
                        }
                       // mapper.TryCreateMap(entityType, dto);
                    }
                }
            }
        }
        //mapper.Build();
        return this;
    }
}
