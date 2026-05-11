using Asp.Versioning;
using Lms.Application.Features.Themes.Commands.CreateTheme;
using Lms.Application.Features.Themes.Commands.DeleteTheme;
using Lms.Application.Features.Themes.Commands.UpdateTheme;
using Lms.Application.Features.Themes.Dtos;
using Lms.Application.Features.Themes.Queries.GetThemes;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/themes")]
    public class ThemesController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ThemeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all themes.")]
        [EndpointDescription("Returns all themes.")]
        [EndpointName("GetThemes")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetThemesQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new theme.")]
        [EndpointDescription("This endpoint takes the name of an theme, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateThemeCommand(name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{themeId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an theme.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid themeId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteThemeCommand(themeId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{themeId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an theme.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid themeId, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateThemeCommand(themeId, name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
