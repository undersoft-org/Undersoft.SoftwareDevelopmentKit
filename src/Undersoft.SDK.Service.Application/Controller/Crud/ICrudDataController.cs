using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Application.Controller.Crud;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

public interface ICrudDataController<TKey, TEntity, TDto> where TDto : class, IDataObject
{
    [HttpGet]
    Task<IActionResult> Count();
    [HttpDelete]
    Task<IActionResult> Delete([FromBody] TDto[] dtos);
    [HttpDelete("{key}")]
    Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TDto dto);
    [HttpGet]
    Task<IActionResult> Get();
    [HttpGet("{offset}/{limit}")]
    Task<IActionResult> Get([FromRoute] int offset, [FromRoute] int limit);
    [HttpGet("{key}")]
    Task<IActionResult> Get([FromRoute] TKey key);
    [HttpPatch]
    Task<IActionResult> Patch([FromBody] TDto[] dtos);
    [HttpPatch("{key}")]
    Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TDto dto);
    [HttpPost("{offset}/{limit}")]
    Task<IActionResult> Post([FromRoute] int offset, [FromRoute] int limit, [FromBody] QuerySet query);
    [HttpPost]
    Task<IActionResult> Post([FromBody] TDto[] dtos);
    [HttpPost("{key}")]
    Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TDto dto);
    [HttpPut]
    Task<IActionResult> Put([FromBody] TDto[] dtos);
    [HttpPut("{key}")]
    Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TDto dto);
}