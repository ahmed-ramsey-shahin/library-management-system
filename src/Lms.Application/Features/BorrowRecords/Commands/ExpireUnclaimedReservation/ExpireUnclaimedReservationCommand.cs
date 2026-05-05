using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.ExpireUnclaimedReservation
{
    public sealed record ExpireUnclaimedReservationCommand : IRequest;
}
