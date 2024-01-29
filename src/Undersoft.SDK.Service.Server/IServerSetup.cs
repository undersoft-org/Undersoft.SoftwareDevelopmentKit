using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Undersoft.SDK.Service.Server
{
    using Undersoft.SDK.Service.Data.Store;

    public partial interface IServerSetup : IServiceSetup
    {
        IServerSetup AddDataServer<TServiceStore>(
            DataServerTypes dataServiceTypes,
            Action<DataServerBuilder> builder = null
        ) where TServiceStore : IDataStore;
        IServerSetup AddAccessServer<TContext>() where TContext : DbContext;
        IServiceSetup AddRepositorySources();
        IServerSetup AddAuthentication();
        IServerSetup AddAuthorization();
        IServerSetup UseServiceClients();
        IServerSetup AddApiVersions(string[] apiVersions);
        IServerSetup ConfigureServer(bool includeSwagger, Type[] sourceTypes = null, Type[] clientTypes = null);
    }
}
