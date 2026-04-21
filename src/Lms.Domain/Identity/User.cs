using Lms.Domain.Circulation;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public sealed class User : EventfulEntity
    {
        public Guid Id { get; }
        public string Email { get; private set; } = string.Empty;
        public Role Role { get; private set; } = Role.Member;
        public UserStatus Status { get; private set; } = UserStatus.Active;
        private readonly List<LibrarianCategory> _librarianCategories = [];
        public IReadOnlyCollection<LibrarianCategory> LibrarianCategories => _librarianCategories.AsReadOnly();
        private readonly List<BorrowRecord> _borrowRecords = [];
        public IReadOnlyCollection<BorrowRecord> BorrowRecords => _borrowRecords.AsReadOnly();
        private readonly List<Fine> _fines = [];
        public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

        private User()
        {
        }

        private User(Guid id, string email, Role role=Role.Member, UserStatus status=UserStatus.Active)
        {
            Id = id;
            Email = email;
            Role = role;
            Status = status;
        }

        public static Result<User> Create(Guid id, string email, Role role=Role.Member, UserStatus status=UserStatus.Active)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(UserErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(UserErrors.EmailRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new User(id, email, role, status);
        }

        public Result<Updated> Update(string email, Role role=Role.Member)
        {
            List<Error> errors = [];

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(UserErrors.EmailRequired);
            }

            if (Role != Role.Member && role == Role.Member)
            {
                errors.Add(UserErrors.RoleInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            Email = email;
            Role = role;

            return Result.Updated;
        }

        public Result<Updated> Suspend()
        {
            Status = UserStatus.Suspended;
            return Result.Updated;
        }

        public Result<Updated> Activate()
        {
            Status = UserStatus.Active;
            return Result.Updated;
        }

        public Result<Deleted> Delete(bool hasActiveBorrows, bool hasUnpaidFines)
        {
            if (Role == Role.Member && (hasActiveBorrows || hasUnpaidFines))
            {
                return UserErrors.MemberDeletionFailed;
            }

            IsDeleted = true;
            return Result.Deleted;
        }

        public Result<Updated> AddCategory(LibrarianCategory category)
        {
            if (Role != Role.Librarian)
            {
                return UserErrors.NotLibrarian;
            }

            _librarianCategories.Add(category);
            return Result.Updated;
        }

        public Result<bool> CanBorrow(int activeBorrows, int maxActiveBorrows, int unpaidFines, int maxUnpaidFines, int lateBorrows, int maxLateBorrows)
        {
            if (Role != Role.Member)
            {
                return UserErrors.NotMember;
            }

            List<Error> errors = [];

            if (activeBorrows > maxActiveBorrows)
            {
                errors.Add(UserErrors.MaxActiveBorrowsReached(maxActiveBorrows));
            }

            if (unpaidFines > maxUnpaidFines)
            {
                errors.Add(UserErrors.MaxUnpaidFines(maxUnpaidFines));
            }

            if (lateBorrows > maxLateBorrows)
            {
                errors.Add(UserErrors.MaxLateBorrows(maxLateBorrows));
            }

            if (Status == UserStatus.Suspended)
            {
                errors.Add(UserErrors.UserSuspended);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return true;
        }

        public Result<Updated> AddCategory(Guid categoryId)
        {
            if (Role != Role.Librarian)
            {
                return UserErrors.NotLibrarian;
            }

            if (Status == UserStatus.Suspended)
            {
                return UserErrors.UserSuspended;
            }

            if (_librarianCategories.Any(lc => lc.CategoryId == categoryId))
            {
                return UserErrors.CategoryAlreadyAssigned;
            }

            _librarianCategories.Add(new LibrarianCategory(Id, categoryId));
            return Result.Updated;
        }

        public Result<Updated> RemoveCategory(Guid categoryId)
        {
            if (Role != Role.Librarian)
            {
                return UserErrors.NotLibrarian;
            }

            if (Status == UserStatus.Suspended)
            {
                return UserErrors.UserSuspended;
            }

            if (!_librarianCategories.Any(lc => lc.CategoryId == categoryId))
            {
                return UserErrors.CategoryNotAssigned;
            }

            _librarianCategories.RemoveAll(lc => lc.CategoryId == categoryId);
            return Result.Updated;
        }
    }
}
