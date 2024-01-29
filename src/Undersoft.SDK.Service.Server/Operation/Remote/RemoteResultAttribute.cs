using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Undersoft.SDK.Service.Data.Remote.Repository;

namespace Undersoft.SDK.Service.Server.Operation.Remote;

public class RemoteResultAttribute : TypeFilterAttribute
{
    public RemoteResultAttribute() : base(typeof(LinkedResult)) { Order = 1; }

    class LinkedResult : IResultFilter
    {
        readonly IRemoteSynchronizer synchronizer;

        public LinkedResult(IServicer servicer) { synchronizer = servicer.GetService<IRemoteSynchronizer>(); }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            synchronizer.AcquireResult();
            IActionResult result = context.Result;
            synchronizer.ReleaseResult();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            IActionResult preresult = context.Result;
        }
    }
}

