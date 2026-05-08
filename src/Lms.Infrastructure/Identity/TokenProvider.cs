using Lms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Lms.Infrastructure.Identity
{
    public class TokenProvider(IConfiguration configuration, IAppDbContext db) : IPasswordHasher
    {
        public string Hash(string password)
        {
            throw new NotImplementedException();
        }

        public bool Verify(string password, string hash)
        {
            throw new NotImplementedException();
        }
    }
}
