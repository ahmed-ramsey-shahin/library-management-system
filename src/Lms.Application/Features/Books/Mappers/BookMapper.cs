using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Books.Mappers
{
    public static class BookMapper
    {
        public static BookSummaryDto ToSummaryDto(this Book book, int availableCopies)
        {
            return new()
            {
                BookId = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Edition = book.Edition,
                AvailableCopies = availableCopies
            };
        }
    }
}
