using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service.Data.File.Blob
{
    public interface IBlobProviderSelector
    {
        IBlobProvider Get([DisallowNull] string containerName);
    }
}