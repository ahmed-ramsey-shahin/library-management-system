using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.GetLibrarianById
{
    public sealed class GetLibrarianByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetLibrarianByIdQueryHandler> logger
    ) : IRequestHandler<GetLibrarianByIdQuery, Result<LibrarianDto>>
    {
        public async Task<Result<LibrarianDto>> Handle(GetLibrarianByIdQuery request, CancellationToken cancellationToken)
        {
            var librarian = await db.Users
                .AsNoTracking()
                .Where(user => user.Role == Role.Librarian && user.Id == request.LibrarianId)
                .Select(librarian => new LibrarianDto
                {
                    LibrarianId = librarian.Id,
                    Email = librarian.Email,
                    FirstName = librarian.FirstName,
                    LastName = librarian.LastName,
                    Address = librarian.Address,
                    LibraryCardNumber = librarian.LibraryCardNumber,
                    Status = librarian.Status,
                    Categories = librarian.LibrarianCategories.Select(librarianCategory => new CategoryDto
                    {
                        CategoryId = librarianCategory.CategoryId,
                        Name = librarianCategory.Category.Name
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (librarian is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not return librarian {LibrarianId}. No librarian was found with ID {LibrarianId}.", request.LibrarianId, request.LibrarianId);
                }

                return ApplicationErrors.UserNotFound;
            }

            return librarian;
        }
    }
}
