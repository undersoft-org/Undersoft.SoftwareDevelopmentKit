using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Series;
using System;
using System.Linq;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Host;
using Undersoft.SDK.Service.Data.Repository;
using System.ComponentModel.Design;

namespace Undersoft.SDK.Service.Host
{
    public partial class ServiceHostSetup : IServiceHostSetup
    {
        public static bool basicProvider;

        IHostBuilder app;
        IHostEnvironment env;

        public ServiceHostSetup(IHostBuilder application) { app = application; }

        public ServiceHostSetup(IHostBuilder application, IHostEnvironment environment, bool useSwagger)
        {
            app = application;
            env = environment; 
        }

        public ServiceHostSetup(IHostBuilder application, IHostEnvironment environment, string[] apiVersions = null)
        {
            app = application;
            env = environment;
        }

        public virtual IServiceHostSetup RebuildProviders()
        {
            if (!basicProvider)
                UseAdvancedProvider();
            else
                UseBasicProvider();
            return this;
        }

        public IServiceHostSetup UseDataServices()
        {
            this.LoadOpenDataEdms().ConfigureAwait(true);
            return this;
        }

        public IServiceHostSetup UseDataMigrations()
        {
            using (var session = ServiceManager.NewSession())
            {
                try
                {
                    IServicer us = session.ServiceProvider.GetRequiredService<IServicer>();
                    us.GetSources().ForEach(e => ((DbContext)e.Context).Database.Migrate());
                }
                catch (Exception ex)
                {
                    this.Error<Applog>("Object migration initial create - unable to connect the database engine", null, ex);
                }
            }

            return this;
        }

        public IServiceHostSetup UseBasicProvider()
        {
            app.UseDefaultServiceProvider(opt => opt.ValidateOnBuild = true);            
            basicProvider = true;
            return this;
        }

        public virtual IServiceHostSetup UseAdvancedProvider()
        {
            ServiceManager.GetRegistry().MergeServices();
            app.UseServiceProviderFactory(ServiceManager.GetProviderFactory());
            basicProvider = false;
            return this;
        }      
    }
}