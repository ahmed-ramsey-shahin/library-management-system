using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Authors.Commands.DeleteAuthor
{
    public sealed class DeleteAuthorCommandHandler(
        ILogger<DeleteAuthorCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteAuthorCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await db.Authors.FirstOrDefaultAsync(author => request.AuthorId == author.Id, cancellationToken);

            if (author is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find author with ID {AuthorId}", request.AuthorId);
                }

                return ApplicationErrors.AuthorNotFound;
            }

            var associatedBooks = await db.BookAuthors.CountAsync(bg => bg.AuthorId == request.AuthorId, cancellationToken);
            var deletionResult = author.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("author", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted author {AuthorId}", request.AuthorId);
            }

            return Result.Deleted;
        }
    }
}
