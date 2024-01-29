using FluentValidation;
using MediatR;
using Undersoft.SDK.Service.Server.Operation.Command;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Server.Behaviour
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandSet, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TResponse>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TResponse>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                (await Task.WhenAll(_validators
                              .Select(v => v
                              .ValidateAsync(context, cancellationToken))))
                                 .SelectMany(r => r.Errors)
                                 .ForEach(f => request.Commands
                                 .ElementAt((int)f
                                    .FormattedMessagePlaceholderValues
                                     ["CollectionIndex"])
                                    .Result.Errors.Add(f));
            }

            return await next();
        }
    }
}