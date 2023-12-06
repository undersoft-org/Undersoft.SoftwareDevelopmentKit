namespace Undersoft.SDK.Service.Data.File.Blob.Container
{
    public static class BlobContainerConfigurationProviderExtensions
    {
        public static BlobContainerConfiguration Get<TContainer>(
            this IBlobContainerConfigurationProvider configurationProvider)
        {
            return configurationProvider.Get(BlobContainerNameAttribute.GetContainerName<TContainer>());
        }
    }
}