using System.IO;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.File.Blob
{
    public interface IBlobProvider
    {
        Task SaveAsync(BlobProviderSaveArgs args);

        Task<bool> DeleteAsync(BlobProviderArgs args);

        Task<bool> ExistsAsync(BlobProviderArgs args);

        Task<Stream> GetOrNullAsync(BlobProviderArgs args);
    }
}