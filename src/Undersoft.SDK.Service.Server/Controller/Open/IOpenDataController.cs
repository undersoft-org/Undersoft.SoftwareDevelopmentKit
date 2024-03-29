﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Microsoft.AspNetCore.OData.Query;

using Undersoft.SDK.Service.Data.Object;
using Uniques;

public interface IOpenDataController<TKey, TEntity, TDto>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
{
    Task<IActionResult> Delete([FromODataUri] TKey key);

    [EnableQuery]
    Task<IQueryable<TDto>> Get();

    [EnableQuery]
    Task<UniqueOne<TDto>> Get([FromODataUri] TKey key);
    Task<IActionResult> Patch([FromODataUri] TKey key, TDto dto);
    Task<IActionResult> Post(TDto dto);
    Task<IActionResult> Put([FromODataUri] TKey key, TDto dto);
}
