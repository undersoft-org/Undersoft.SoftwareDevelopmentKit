using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Server.Behaviour;

using Logging;
using Operation;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IOperation
    where TResponse : IOperation
{
    //private ActivitySource activitySource;

    //public LoggingBehaviour(Instrumentation instrumentation)
    //{
    //    activitySource = instrumentation.ActivitySource;
    //}

    public LoggingBehaviour()
    {        
    }

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
       )
    {

        //using var activity = activitySource.StartActivity($"Operation Request: {request.GetType().Name}");

        request.Info<Apilog>($"Request data source", request.Input);

        var response = await next();

        response.Info<Apilog>($"Response data result", response.Output);

        return response;
    }
}
