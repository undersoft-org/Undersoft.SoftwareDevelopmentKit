using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Repository.Client.Remote;
using Undersoft.SDK.Service.Data.Entity;
using Undersoft.SDK.Service.Data.Service.Remote;

namespace Undersoft.SDK.Service.Host
{
    public partial class ServiceHostSetup
    {
        public static void AddOpenDataServiceImplementations()
        {
            IServiceManager sm = ServiceManager.GetManager();
            IServiceRegistry service = sm.Registry;
            HashSet<Type> duplicateCheck = new HashSet<Type>();
            Type[] stores = new Type[] { typeof(IDataStore) };

            /**************************************** DataService Entity Type Routines ***************************************/
            foreach (ISeries<IEdmEntityType> contextEntityTypes in OpenDataServiceRegistry.ContextEntities)
            {
                foreach (IEdmEntityType _entityType in contextEntityTypes)
                {
                    Type entityType = OpenDataServiceRegistry.GetMappedType(_entityType.Name);

                    if (duplicateCheck.Add(entityType))
                    {
                        Type callerType = DataStoreRegistry.GetRemoteType(entityType.FullName);

                        /*****************************************************************************************/
                        foreach (Type store in stores)
                        {
                            if ((entityType != null))
                            {
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(IRemoteRepository<,>).MakeGenericType(store, entityType),
                                    typeof(RemoteRepository<,>).MakeGenericType(store, entityType));

                                service.AddScoped(
                                    typeof(IEntityCache<,>).MakeGenericType(store, entityType),
                                    typeof(EntityCache<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(IRemoteSet<,>).MakeGenericType(store, entityType),
                                    typeof(RemoteSet<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                if (callerType != null)
                                {
                                    /*********************************************************************************************/
                                    service.AddScoped(
                                        typeof(IRepositoryLink<,,>).MakeGenericType(store, callerType, entityType),
                                        typeof(RepositoryLink<,,>).MakeGenericType(store, callerType, entityType));

                                    service.AddScoped(
                                        typeof(IRemoteObject<,>).MakeGenericType(store, callerType),
                                        typeof(RepositoryLink<,,>).MakeGenericType(store, callerType, entityType));
                                    /*********************************************************************************************/
                                }
                            }
                        }
                    }
                }
            }
            //app.RebuildProviders();
        }
    }
}