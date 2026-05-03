using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.GetAdminById
{
    public sealed class GetAdminByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetAdminByIdQueryHandler> logger
    ) : IRequestHandler<GetAdminByIdQuery, Result<AdminDto>>
    {
        public async Task<Result<AdminDto>> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
        {
            var admin = await db.Users
                .AsNoTracking()
                .Where(user => user.Role == Role.Admin && user.Id == request.AdminId)
                .Select(admin => new AdminDto
                {
                    AdminId = admin.Id,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Address = admin.Address,
                    LibraryCardNumber = admin.LibraryCardNumber,
                    Status = admin.Status
                }).FirstOrDefaultAsync(cancellationToken);

            if (admin is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not return admin {AdminId}. No admin was found with ID {AdminId}.", request.AdminId, request.AdminId);
                }

                return ApplicationErrors.UserNotFound;
            }

            return admin;
        }
    }
}
