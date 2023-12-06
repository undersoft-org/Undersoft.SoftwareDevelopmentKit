using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Service.Data.Store;
using System.Reflection;
using Undersoft.SDK.Service.Application.Controller;
using Undersoft.SDK.Service.Application.Account;
using Microsoft.AspNetCore.OData.Formatter;

using Undersoft.SDK.Service.Data.Object;

namespace Undersoft.SDK.Service.Application.DataServer;

public class OpenDataServerBuilder<TStore> : DataServerBuilder, IDataServerBuilder<TStore> where TStore : IDataServiceStore
{
    protected ODataConventionModelBuilder odataBuilder;
    protected IEdmModel edmModel;
    protected static bool actionSetAdded;

    public OpenDataServerBuilder() : base()
    {
        odataBuilder = new ODataConventionModelBuilder();
        StoreType = typeof(TStore);
    }

    public OpenDataServerBuilder(string routePrefix, int pageLimit) : this()
    {
        RoutePrefix += "/" + routePrefix;
        PageLimit = pageLimit;
    }

    public override void Build()
    {
        BuildEdm();
    }

    public object EntitySet(Type entityType)
    {
        var entitySetName = entityType.Name;
        if (entityType.IsGenericType && entityType.IsAssignableTo(typeof(Identifier)))
            entitySetName =
                entityType.GetGenericArguments().FirstOrDefault().Name + "Identifier";

        var etc = odataBuilder.AddEntityType(entityType);
        etc.Name = entitySetName;
        var ets = odataBuilder.AddEntitySet(entitySetName, etc);
        ets.EntityType.HasKey(entityType.GetProperty("Id"));

        return ets;
    }

    public object EntitySet<TDto>() where TDto : class
    {
        return odataBuilder.EntitySet<TDto>(typeof(TDto).Name);
    }

    public IEdmModel GetEdm()
    {
        if (edmModel == null)
        {
            AddActionSet();
            edmModel = odataBuilder.GetEdmModel();
            odataBuilder.ValidateModel(edmModel);
        }
        return edmModel;
    }

    public void BuildEdm()
    {
        Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
        var controllerTypes = asm.SelectMany(
                a =>
                    a.GetTypes()
                        .Where(
                            type => type.GetCustomAttribute<OpenDataServiceAttribute>()
                                    != null || type.GetCustomAttribute<OpenDataActionServiceAttribute>() != null
                        )
                        ).ToArray();

        foreach (var types in controllerTypes)
        {
            var genTypes = types.BaseType.GenericTypeArguments;

            if (genTypes.Length > 4 && genTypes[1].IsAssignableTo(StoreType) && genTypes[2].IsAssignableTo(StoreType))
                EntitySet(genTypes[4]);
            else if (genTypes.Length > 3)
            {
                if (genTypes[3].IsAssignableTo(typeof(IDataObject)) && genTypes[1].IsAssignableTo(StoreType))
                    EntitySet(genTypes[3]);
                else
                    continue;
            }
            else if (genTypes.Length > 2)
                if (genTypes[2].IsAssignableTo(typeof(IDataObject)) && genTypes[0].IsAssignableTo(StoreType))
                    EntitySet(genTypes[2]);
                else
                    continue;
        }
    }

    public IMvcBuilder AddODataServicer(IMvcBuilder mvc)
    {
        var model = GetEdm();
        var route = GetRoutes();
        mvc.AddOData(b =>
        {
            b.RouteOptions.EnableQualifiedOperationCall = true;
            b.RouteOptions.EnableUnqualifiedOperationCall = true;
            b.RouteOptions.EnableKeyInParenthesis = true;
            b.RouteOptions.EnableKeyAsSegment = false;
            b.RouteOptions.EnableControllerNameCaseInsensitive = true;
            b.EnableQueryFeatures(PageLimit).AddRouteComponents(route, model);

        });
        AddODataSupport(mvc);
        return mvc;
    }

    private IMvcBuilder AddODataSupport(IMvcBuilder mvc)
    {
        mvc.AddMvcOptions(options =>
        {
            foreach (
                OutputFormatter outputFormatter in options.OutputFormatters
                    .OfType<OutputFormatter>()
                    .Where(x => x.SupportedMediaTypes.Count == 0)
            )
            {
                outputFormatter.SupportedMediaTypes.Add(
                    new MediaTypeHeaderValue("application/prs.odatatestxx-odata")
                );
            }

            foreach (
                InputFormatter inputFormatter in options.InputFormatters
                    .OfType<InputFormatter>()
                    .Where(x => x.SupportedMediaTypes.Count == 0)
            )
            {
                inputFormatter.SupportedMediaTypes.Add(
                    new MediaTypeHeaderValue("application/prs.odatatestxx-odata")
                );
            }
        });
        return mvc;
    }

    protected override string GetRoutes()
    {
        if (StoreType == typeof(IEventStore))
        {
            return StoreRoutes.OpenEventStore;
        }
        else if (StoreType == typeof(IIdentityStore))
        {
            return StoreRoutes.OpenIdentityStore;
        }
        else
        {
            return StoreRoutes.OpenDataStore;
        }
    }

    public override IDataServerBuilder AddIdentityActionSet()
    {
        AddActionSet();
        return base.AddIdentityActionSet();
    }

    public void AddActionSet()
    {
        if (actionSetAdded)
            return;

        odataBuilder.EntityType<Account>().Action("SignIn")
            .Returns<string>()
            .Parameter<Credentials>("Credentials");

        odataBuilder.EntityType<Account>().Action("SignUp")
            .Returns<string>()
            .Parameter<Credentials>("Credentials");

        odataBuilder.EntityType<Account>().Action("SignOut")
            .Returns<string>()
            .Parameter<Credentials>("Credentials");

        odataBuilder.EntityType<Account>().Action("ResetPassword")
            .Returns<string>()
            .Parameter<Credentials>("Credentials");

        odataBuilder.EntityType<Account>().Action("CompleteRegistration")
            .Returns<string>()
            .Parameter<Credentials>("Credentials");

        actionSetAdded = true;
    }

}
