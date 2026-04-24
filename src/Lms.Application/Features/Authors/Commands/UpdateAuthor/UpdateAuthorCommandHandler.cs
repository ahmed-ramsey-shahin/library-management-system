using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Authors.Commands.UpdateAuthor
{
    public sealed class UpdateAuthorCommandHandler(
        ILogger<UpdateAuthorCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateAuthorCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await db.Authors.FirstOrDefaultAsync(author => author.Id == request.AuthorId, cancellationToken);

            if (author is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find author with ID {AuthorId}", request.AuthorId);
                }

                return ApplicationErrors.AuthorNotFound;
            }

            var exists = await db.Authors.AnyAsync(author => string.Equals(author.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Author creation aborted. Author already exists");
                }

                return ApplicationErrors.AuthorAlreadyExists;
            }

            author.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("author", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Author {AuthorId} updated successfully.", author.Id);
            }

            return Result.Updated;
        }
    }
}
