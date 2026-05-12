using Asp.Versioning;
using Lms.Application.Features.Users.Dtos;
using Lms.Application.Features.Users.Queries.GenerateTokens;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("identity")]
    [ApiVersionNeutral]
    public class IdentityController(ISender sender) : ApiController
    {
        [HttpPost("token/generate")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Generates an access and refresh token for a valid user.")]
        [EndpointDescription("Authenticates a user using provided credentials and returns a JWT token pair.")]
        [EndpointName("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokensQuery request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(request, cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
