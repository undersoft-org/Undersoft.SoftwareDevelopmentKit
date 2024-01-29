namespace Undersoft.SDK.Service.Application.Components;

public interface IFilter
{
    [NotNull]
    IFilterAction? FilterAction { get; set; }
}
