using System.ServiceModel;

namespace Undersoft.SDK.Service.Application.Controller.Stream;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

[ServiceContract]
public interface IStreamDataController<TDto> where TDto : class, IDataObject
{
    Task<int> Count();
    IAsyncEnumerable<TDto> All();
    IAsyncEnumerable<TDto> Range(int offset, int limit);
    IAsyncEnumerable<TDto> Query(int offset, int limit, QuerySet query);
    IAsyncEnumerable<string> Creates(TDto[] dtos);
    IAsyncEnumerable<string> Changes(TDto[] dtos);
    IAsyncEnumerable<string> Updates(TDto[] dtos);
    IAsyncEnumerable<string> Deletes(TDto[] dtos);
}