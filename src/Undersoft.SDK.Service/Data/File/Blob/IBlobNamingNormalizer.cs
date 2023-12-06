namespace Undersoft.SDK.Service.Data.File.Blob
{
    public interface IBlobNamingNormalizer
    {
        string NormalizeContainerName(string containerName);

        string NormalizeBlobName(string blobName);
    }
}