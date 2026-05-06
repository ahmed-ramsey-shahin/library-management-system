namespace Lms.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string message, CancellationToken cancellationToken);
    }
}
