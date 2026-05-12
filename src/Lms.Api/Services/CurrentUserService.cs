using System.Security.Claims;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Identity;

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

        public Role? UserRole
        {
            get
            {
                var userRole = httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                if (Enum.TryParse(userRole, out Role role))
                {
                    return role;
                }
                return null;
            }
        }
    }
}
