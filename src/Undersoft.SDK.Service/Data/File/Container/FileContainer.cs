namespace Undersoft.SDK.Service.Data.File.Container;

using Blob;
using Blob.Container;
using Blob.FileSystem;

public class FileContainer : BlobContainer
{
    public FileContainer(string containerName)
        : this(containerName,
        new BlobContainerConfiguration(),
        new FileSystemBlobProvider(
            new DefaultBlobFilePathCalculator()))
    {
    }

    public FileContainer(
        string containerName,
        BlobContainerConfiguration configuration,
        IBlobProvider provider,
        IBlobNormalizeNamingService blobNormalizeNamingService = null)
        : base(containerName, configuration, provider, blobNormalizeNamingService)
    {
        configuration.UseFileSystem((c) => { c.BasePath = "../.store"; c.AppendContainerNameToBasePath = true; });
    }

    public DataFile Get(string filename) => new DataFile(this, filename);
}
