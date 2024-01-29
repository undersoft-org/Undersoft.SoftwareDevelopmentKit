using Microsoft.Extensions.Caching.Memory;

namespace Undersoft.SDK.Service.Application.Components;

public interface ICacheManager
{
    TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory);

    Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);

    void SetStartTime();

    DateTimeOffset GetStartTime();

    void Clear(string? key = null);
}
