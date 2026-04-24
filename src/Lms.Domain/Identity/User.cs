using Lms.Domain.Circulation;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public sealed class User : EventfulEntity
    {
        public Guid Id { get; }
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string LibraryCardNumber { get; private set; } = string.Empty;
        public Role Role { get; private set; } = Role.Member;
        public UserStatus Status { get; private set; } = UserStatus.Active;
        public string Password { get; private set; } = string.Empty;
        public string Salt { get; private set; } = string.Empty;
        private readonly List<LibrarianCategory> _librarianCategories = [];
        public IReadOnlyCollection<LibrarianCategory> LibrarianCategories => _librarianCategories.AsReadOnly();
        private readonly List<RefreshToken> _refreshTokens = [];
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private User()
        {
        }

        private User(
            Guid id,
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            string address,
            string libraryCardNumber,
            string passwordHash,
            string salt,
            Role role=Role.Member,
            UserStatus status=UserStatus.Active
        )
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Address = address;
            LibraryCardNumber = libraryCardNumber;
            Role = role;
            Status = status;
            Password = passwordHash;
            Salt = salt;
        }

        public static Result<User> Create(
            Guid id,
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            string address,
            string libraryCardNumber,
            string passwordHash,
            string salt,
            Role role=Role.Member,
            UserStatus status=UserStatus.Active
        )
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

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                errors.Add(UserErrors.PasswordRequired);
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                errors.Add(UserErrors.SaltRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(UserErrors.FirstNameRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(UserErrors.LastNameRequired);
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(UserErrors.PhoneNumberRequired);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                errors.Add(UserErrors.AddressRequired);
            }

            if (string.IsNullOrWhiteSpace(libraryCardNumber))
            {
                errors.Add(UserErrors.LibraryCardNumberRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new User(id, email, firstName, lastName, phoneNumber, address, libraryCardNumber, passwordHash, salt, role, status);
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

        public Result<Updated> ChangePassword(string passwordHash, string salt)
        {
            List<Error> errors = [];

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                errors.Add(UserErrors.PasswordRequired);
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                errors.Add(UserErrors.SaltRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            Password = passwordHash;
            Salt = salt;
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

        public Result<bool> CanBorrow(MemberBorrowState state, LibraryPolicy policy)
        {
            if (Role != Role.Member)
            {
                return UserErrors.NotMember;

            }
            List<Error> errors = [];

            if (state.ActiveBorrows > policy.MaxActiveBorrows)
            {
                errors.Add(UserErrors.MaxActiveBorrowsReached(policy.MaxActiveBorrows));
            }

            if (state.UnpaidFines > policy.MaxUnpaidFines)
            {
                errors.Add(UserErrors.MaxUnpaidFines(policy.MaxUnpaidFines));
            }

            if (state.LateBorrows > policy.MaxLateBorrows)
            {
                errors.Add(UserErrors.MaxLateBorrows(policy.MaxLateBorrows));
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

        public Result<Updated> ChangePersonalDetails(string firstName, string lastName, string phoneNumber, string address)
        {
            List<Error> errors = [];

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(UserErrors.FirstNameRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(UserErrors.LastNameRequired);
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add(UserErrors.PhoneNumberRequired);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                errors.Add(UserErrors.AddressRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Address = address;
            return Result.Updated;
        }

        public Result<Updated> UpdateLibraryCardNumber(string libraryCardNumber)
        {
            if (string.IsNullOrWhiteSpace(libraryCardNumber))
            {
                return UserErrors.LibraryCardNumberRequired;
            }

            LibraryCardNumber = libraryCardNumber;
            return Result.Updated;
        }

        public Result<Updated> RevokeAllTokens()
        {
            foreach (var token in _refreshTokens)
            {
                var revokeResult = token.Revoke();
                if (revokeResult.IsError)
                {
                    return revokeResult.Errors!;
                }
            }

            return Result.Updated;
        }
    }
}
