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
        private readonly List<LibrarianCategory> _librarianCategories = [];
        public IReadOnlyCollection<LibrarianCategory> LibrarianCategories => _librarianCategories.AsReadOnly();
        private readonly List<RefreshToken> _refreshTokens = [];
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        // readonly navigation poperties
        private readonly List<BorrowRecord> _borrowRecords = [];
        public IReadOnlyCollection<BorrowRecord> BorrowRecords => _borrowRecords.AsReadOnly();
        private readonly List<Fine> _fines = [];
        public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

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

            return new User(id, email, firstName, lastName, phoneNumber, address, libraryCardNumber, passwordHash, role, status);
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

        public Result<Updated> ChangePassword(string passwordHash)
        {
            List<Error> errors = [];

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                errors.Add(UserErrors.PasswordRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            Password = passwordHash;
            return Result.Updated;
        }

        public Result<Updated> Suspend()
        {
            if (Status == UserStatus.Suspended)
            {
                return Result.Updated;
            }

            Status = UserStatus.Suspended;
            AddEvent(new UserSuspended(Id));
            return Result.Updated;
        }

        public Result<Updated> Activate()
        {
            if (Status == UserStatus.Active)
            {
                return Result.Updated;
            }

            Status = UserStatus.Active;
            AddEvent(new UserActivated(Id));
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
            AddEvent(new CategoryAdded(Id, categoryId));
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
            AddEvent(new CategoryRemoved(Id, categoryId));
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

        public static string GenerateLibraryNumber()
        {
            var year = DateTimeOffset.UtcNow.ToString("yy");
            var randomDigits = Random.Shared.Next(1000000, 9999999).ToString();
            var baseNumber = year + randomDigits;
            var checkDigit = CalculateLuhnCheckDigit(baseNumber);
            return baseNumber + checkDigit;
        }

        public static string CalculateLuhnCheckDigit(string number)
        {
            int sum = 0;
            bool alternate = true;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(number[i].ToString());

                if (alternate)
                {
                    n *= 2;

                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit.ToString();
        }

        public static bool IsValidLibraryNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 10)
            {
                return false;
            }

            if (!cardNumber.All(char.IsDigit))
            {
                return false;
            }

            var alternate = false;
            var sum = 0;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    n *= 2;

                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }
    }
}
