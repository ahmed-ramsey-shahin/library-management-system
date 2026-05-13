using System.Net.Http.Json;
using Lms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Lms.Infrastructure.Services
{
    public class EmailService(IHttpClientFactory clientFactory, IConfiguration configuration) : IEmailService
    {
        private readonly HttpClient _client = clientFactory.CreateClient("EmailClient");
        private readonly string _senderName = configuration["EmailService:SenderName"]!;
        private readonly string _senderEmail = configuration["EmailService:SenderEmail"]!;

        public async Task SendEmailAsync(string recipientEmail, string recipientName, string subjectTxt, string messageTxt, CancellationToken cancellationToken)
        {
            var payload = new
            {
                sender = new
                {
                    name = _senderName,
                    email = _senderEmail,
                },
                to = new[]
                {
                    new
                    {
                        email = recipientEmail,
                        name = recipientName,
                    }
                },
                subject = subjectTxt,
                textContent = messageTxt
            };
            var response = await _client.PostAsJsonAsync("smtp/email", payload, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
