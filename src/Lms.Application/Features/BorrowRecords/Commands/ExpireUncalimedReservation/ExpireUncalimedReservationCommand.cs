using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.ExpireUncalimedReservation
{
    public sealed record ExpireUncalimedReservationCommand : IRequest;
}
