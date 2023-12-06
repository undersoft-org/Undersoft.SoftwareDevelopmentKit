using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using Undersoft.SDK.Service.Data.File.Blob.Container;

namespace Undersoft.SDK.Service.Data.File.Blob
{
    public static class BlobProviderSelectorExtensions
    {
        public static IBlobProvider Get<TContainer>(
            [DisallowNull] this IBlobProviderSelector selector)
        {
            return selector.Get(BlobContainerNameAttribute.GetContainerName<TContainer>());
        }
    }
}