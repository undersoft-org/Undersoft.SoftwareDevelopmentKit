using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository.Source
{
    public interface ISourceProviderConfiguration
    {
        IServiceRegistry AddSourceProvider(StoreProvider provider);
        DbContextOptionsBuilder BuildOptions(DbContextOptionsBuilder builder, StoreProvider provider, string connectionString);
    }
}