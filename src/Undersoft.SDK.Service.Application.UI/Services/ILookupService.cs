namespace Undersoft.SDK.Service.Application.Components;

public interface ILookupService
{
    IEnumerable<SelectedItem>? GetItemsByKey(string? key);
}
