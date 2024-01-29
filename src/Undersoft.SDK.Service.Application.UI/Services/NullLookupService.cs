using Undersoft.SDK.Service.Application.Components;

namespace Undersoft.SDK.Service.Application.Services;

internal class NullLookupService : ILookupService
{
    public IEnumerable<SelectedItem>? GetItemsByKey(string? key) => null;
}
