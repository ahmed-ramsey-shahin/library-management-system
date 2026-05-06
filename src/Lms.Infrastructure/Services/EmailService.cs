using Lms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Resend;

namespace Lms.Infrastructure.Services
{
    public class EmailService(IConfiguration configuration, IResend client) : IEmailService
    {
        public async Task SendEmailAsync(string recipientEmail, string recipientName, string subjectTxt, string messageTxt, CancellationToken cancellationToken)
        {
            var email = new EmailMessage
            {
                From = configuration["Email:DefaultFrom"]!,
                To = { recipientEmail },
                Subject = subjectTxt,
                TextBody = $"Hello {recipientName}\n{messageTxt}",
            };
            await client.EmailSendAsync(email, cancellationToken);
        }
    }
}
