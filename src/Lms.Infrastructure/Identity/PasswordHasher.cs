using Lms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Lms.Infrastructure.Identity
{
    public class PasswordHasher(IConfiguration configuration) : IPasswordHasher
    {
        public string Hash(string password)
        {
            var pepper = configuration["Security:PasswordPepper"];
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password + pepper, workFactor: 12);
        }

        public bool Verify(string password, string hash)
        {
            var pepper = configuration["Security:PasswordPepper"];
            return BCrypt.Net.BCrypt.EnhancedVerify(password + pepper, hash);
        }
    }
}
