using System;
using Undersoft.SDK.Service.Data.File.Blob.Container;

namespace Undersoft.SDK.Service.Data.File.Blob.FileSystem
{
    public static class FileSystemBlobContainerConfigurationExtensions
    {
        public static FileSystemBlobProviderConfiguration GetFileSystemConfiguration(this BlobContainerConfiguration containerConfiguration)
        {
            return new FileSystemBlobProviderConfiguration(containerConfiguration);
        }

        public static BlobContainerConfiguration UseFileSystem(this BlobContainerConfiguration containerConfiguration,
            Action<FileSystemBlobProviderConfiguration> fileSystemConfigureAction)
        {
            containerConfiguration.ProviderType = typeof(FileSystemBlobProvider);
            containerConfiguration.NamingNormalizers.Add(new FileSystemBlobNamingNormalizer());

            fileSystemConfigureAction(new FileSystemBlobProviderConfiguration(containerConfiguration));

            return containerConfiguration;
        }
    }
}