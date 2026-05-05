using Lms.Application.Features.BorrowRecords.Commands.MarkOverdueBorrowRecords;
using Lms.Application.Features.Fines.Commands.ProcessLateBorrowRecords;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Circulations.Commands.RunDailyProcessing
{
    public sealed class RunDailyProcessingCommandHandler(
        ILogger<RunDailyProcessingCommandHandler> logger,
        ISender sender
    ) : IRequestHandler<RunDailyProcessingCommand>
    {
        public async Task Handle(RunDailyProcessingCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("-- Starting daily library processing. --");
            logger.LogInformation("Phase 1: Marking overdue records.");
            await sender.Send(new MarkOverdueBorrowRecordsCommand(), cancellationToken);
            logger.LogInformation("Phase 2: Assessing daily fines.");
            await sender.Send(new ProcessLateBorrowRecordsCommand(), cancellationToken);
            logger.LogInformation("-- Dail library processing complete. --");
        }
    }
}
