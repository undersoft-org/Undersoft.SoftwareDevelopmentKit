using Undersoft.SDK.Service.Data.File.Blob.Container;

namespace Undersoft.SDK.Service.Data.File.Blob
{
    public class BlobStoringOptions
    {
        public BlobContainerConfigurations Containers { get; }

        public BlobStoringOptions()
        {
            Containers = new BlobContainerConfigurations();
        }
    }
}