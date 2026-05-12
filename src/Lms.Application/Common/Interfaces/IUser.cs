using Lms.Domain.Identity;

namespace Lms.Application.Common.Interfaces
{
    public interface IUser
    {
        Guid? Id { get; }
        Role? UserRole { get; }
    }
}
