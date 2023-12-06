using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Undersoft.SDK.Service.Application;

using Series;
using Operation.Command;
using Operation.Command.Handler;
using Operation.Notification;
using Operation.Notification.Handler;
using ViewModel;

public partial class ApplicationSetup
{
    public IApplicationSetup AddApplicationSetupInternalCQRSActionImplementations(Assembly[] assemblies)
    {
        IServiceRegistry service = registry;

        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        IServiceCollection deck = service
            .AddTransient<ISeries<IViewModel>, Registry<IViewModel>>()
            .AddScoped<ITypedSeries<IViewModel>, TypedRegistry<IViewModel>>();

        var controllerTypes = assemblies
            .SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type =>
                                type.GetCustomAttributes()
                                    .Any(
                                        a =>
                                            a.GetType().IsAssignableTo(typeof(DataActionServiceAttribute))
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

        foreach (var controllerType in controllerTypes)
        {
            var genericTypes = controllerType.BaseType.GenericTypeArguments;
            var store = genericTypes[0];
            var actionType = genericTypes[1];
            var dtoType = genericTypes[2];

            service.AddTransient(
                     typeof(IRequest<>).MakeGenericType(
                         typeof(ActionCommand<>).MakeGenericType(dtoType)
                     ),
                     typeof(ActionCommand<>).MakeGenericType(dtoType)
                 );

            service.AddTransient(
               typeof(IRequestHandler<,>).MakeGenericType(
                   new[]
                   {
                            typeof(Execute<,,>).MakeGenericType(store, actionType, dtoType),
                            typeof(ActionCommand<>).MakeGenericType(dtoType)
                   }
               ),
               typeof(ExecuteHandler<,,>).MakeGenericType(store, actionType, dtoType)
           );

            service.AddTransient(
             typeof(INotificationHandler<>).MakeGenericType(
                 typeof(Executed<,,>).MakeGenericType(store, actionType, dtoType)
             ),
             typeof(ExecutedHandler<,,>).MakeGenericType(store, actionType, dtoType)
            );
        }
        return this;
    }
}
