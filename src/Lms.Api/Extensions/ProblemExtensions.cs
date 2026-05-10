using Lms.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Extensions
{
    public static class ProblemExtensions
    {
        private static IResult ValidationProblem(List<Error> errors)
        {
            var errorsDict = errors.ToDictionary(error => error.Code, error => new[] { error.Description });
            var problemDetails = new ValidationProblemDetails(errorsDict)
            {
                Status = StatusCodes.Status400BadRequest,
            };
            return Results.Json(problemDetails, statusCode: StatusCodes.Status400BadRequest);
        }
        public static IResult ToProblem(this List<Error> errors)
        {
            if (errors.Count == 0)
            {
                return Results.Problem();
            }

            if (errors.All(error => error.Type == ErrorKind.Validation))
            {
                return ValidationProblem(errors);
            }

            return Problem(errors[0]);
        }

        private static IResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                ErrorKind.Validation => StatusCodes.Status400BadRequest,
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Unauthorized => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError,
            };
            return Results.Problem(statusCode: statusCode, title: error.Description);
        }
    }
}
