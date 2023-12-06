using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

namespace Undersoft.SDK.Service.Application.Controller.Open;


using Uniques;

public interface IOpenDataActionController<TKey, TEntity, TDto> where TDto : class
{
    Task<IActionResult> Post([FromODataBody] TDto dto);
}