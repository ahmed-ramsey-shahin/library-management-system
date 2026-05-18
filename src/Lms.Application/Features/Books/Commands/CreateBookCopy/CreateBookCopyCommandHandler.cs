using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.CreateBookCopy
{
    public sealed class CreateBookCopyCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<CreateBookCopyCommandHandler> logger
    ) : IRequestHandler<CreateBookCopyCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBookCopyCommand request, CancellationToken cancellationToken)
        {
            var barcodeExists = await db.BookCopies.AnyAsync(copy => copy.Barcode == request.Barcode, cancellationToken);

            if (barcodeExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy creation aborted. A book copy with barcode {CopyBarcode} already exists.", request.Barcode);
                }

                return ApplicationErrors.BarcodeAlreadyExists;
            }

            var book = await db.Books.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy creation aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var copyStatus = request.Status ?? BookCopyStatus.Good;
            var copyState = request.State ?? BookCopyState.Available;
            var acquisitionDate = request.AcquisitionDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var copyCreationResult = book.AddCopy(
                id: Guid.NewGuid(),
                barcode: request.Barcode,
                location: request.Location,
                acquisitionDate: acquisitionDate,
                status: copyStatus,
                state: copyState
            );

            if (copyCreationResult.IsError)
            {
                return copyCreationResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book-copy", "book"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("A book copy with ID {CopyId} was created.", copyCreationResult.Value.Id);
            }

            return copyCreationResult.Value.Id;
        }
    }
}
