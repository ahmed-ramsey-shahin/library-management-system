using System.Security.Claims;
using Lms.Application.Common.Interfaces;

namespace Lms.Api.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContext) : IUser
    {
        public Guid? Id
        {
            get
            {
                var userIdentifier = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdentifier, out Guid userGuid))
                {
                    return userGuid;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
    }
}
