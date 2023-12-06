using System.Linq;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Repository;

using Host;

public static class RepositoryManagerExtensions
{
    public static async Task LoadOpenDataEdms(this ServiceHostSetup app)
    {
        await Task.Run(() =>
        {
            RepositoryManager.Clients.ForEach((client) =>
            {
                client.BuildMetadata();
            });

            ServiceHostSetup.AddOpenDataServiceImplementations();
            app.RebuildProviders();
        });
    }

}
