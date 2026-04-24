using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Authors.Mappers;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Authors.Commands.CreateAuthor
{
    public sealed class CreateAuthorCommandHandler(
        IAppDbContext db,
        ILogger<CreateAuthorCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateAuthorCommand, Result<AuthorDto>>
    {
        public async Task<Result<AuthorDto>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Authors.AnyAsync(author => string.Equals(author.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Author creation aborted. Author already exists");
                }

                return ApplicationErrors.AuthorAlreadyExists;
            }

            var authorCreationResult = Author.Create(Guid.NewGuid(), request.Name);

            if (authorCreationResult.IsError)
            {
                return authorCreationResult.Errors!;
            }

            db.Authors.Add(authorCreationResult.Value);
            var author = authorCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("author", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Author created successfully. Id: {AuthorId}", author.AuthorId);
            }

            return author;
        }
    }
}
