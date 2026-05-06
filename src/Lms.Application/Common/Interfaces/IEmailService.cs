namespace Lms.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string recipientName, string subjectTxt, string messageTxt, CancellationToken cancellationToken);
    }
}
