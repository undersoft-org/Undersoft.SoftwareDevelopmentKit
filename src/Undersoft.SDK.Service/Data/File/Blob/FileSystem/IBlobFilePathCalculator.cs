namespace Undersoft.SDK.Service.Data.File.Blob.FileSystem
{
    public interface IBlobFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}