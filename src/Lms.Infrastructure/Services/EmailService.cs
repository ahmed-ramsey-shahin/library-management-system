using Lms.Application.Common.Interfaces;

namespace Lms.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
