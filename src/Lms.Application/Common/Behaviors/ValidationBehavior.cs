using FluentValidation;
using Lms.Domain.Common.Results;
using Lms.Domain.Common.Results.Abstractions;
using MediatR;

namespace Lms.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(
        IValidator<TRequest>? validator=null
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : IResult
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validator is null)
            {
                return await next(cancellationToken);
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                return await next(cancellationToken);
            }

            var errors = validationResult.Errors
                .ConvertAll(
                    error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage
                )
            );
            return (dynamic)errors;
        }
    }
}
