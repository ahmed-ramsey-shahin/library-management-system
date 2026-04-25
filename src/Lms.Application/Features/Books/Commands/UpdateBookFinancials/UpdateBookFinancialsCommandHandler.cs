using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookFinancials
{
    public sealed class UpdateBookFinancialsCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<UpdateBookFinancialsCommandHandler> logger
    ) : IRequestHandler<UpdateBookFinancialsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookFinancialsCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Book update aborted. No book was found with ID {BookId}",
                        request.BookId
                    );
                }

                return ApplicationErrors.BookNotFound;
            }

            var borrowPricePerDay = request.BorrowPricePerDay ?? book.BorrowPricePerDay;
            var finePerDay = request.FinePerDay ?? book.FinePerDay;
            var lostFee = request.LostFee ?? book.LostFee;
            var damageFee = request.DamageFee ?? book.DamageFee;
            var result = book.UpdateFinancials(borrowPricePerDay, finePerDay, lostFee, damageFee);

            if (result.IsError)
            {
                return result.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book {BookId} details was updated.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
