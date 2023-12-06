using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Undersoft.SDK.Service.Application.Controller.Open;

using Operation.Command;
using SDK.Service.Data.Store;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Data;

[OpenDataActionService]
public abstract class OpenDataActionController<TStore, TType, TDto>
    : ODataController, IOpenDataActionController<TStore, TType, TDto> where TDto : class
    where TType : class
    where TStore : IDataServiceStore
{
    protected readonly IServicer _servicer;
    protected readonly DataActionKind _kind;

    protected OpenDataActionController() { }

    public OpenDataActionController(
        IServicer servicer,
        DataActionKind kind = DataActionKind.None
    )
    {
        _servicer = servicer;
        _kind = kind;
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(TDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _servicer.Send(new Execute<TStore, TType, TDto>
                                                (_kind, dto))
                                                    .ConfigureAwait(false);
        return !result.IsValid
               ? UnprocessableEntity(result.ErrorMessages)
               : Created(result.Id.ToString());
    }
}
