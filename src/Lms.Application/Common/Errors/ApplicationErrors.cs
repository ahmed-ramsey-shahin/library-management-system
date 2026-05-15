using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        // --- Books ---
        public static Error BookNotFound => Error.NotFound("Book.NotFound", "The requested book was not found.");
        public static Error BookIsbnInvalid => Error.Validation("Book.InvalidIsbn", "The provided ISBN format is invalid.");
        public static Error BookIssnInvalid => Error.Validation("Book.InvalidIssn", "The provided ISSN format is invalid.");
        public static Error BookTitleLength => Error.Validation("Book.TitleTooLong", "The book title cannot exceed 255 characters.");
        public static Error BookDescriptionLength => Error.Validation("Book.DescriptionTooLong", "The book description cannot exceed 1024 characters.");
        public static Error IsbnAlreadyExists => Error.Conflict("Book.IsbnConflict", "A book with this ISBN already exists in the catalog.");
        public static Error IssnAlreadyExists => Error.Conflict("Book.IssnConflict", "A book with this ISSN already exists in the catalog.");

        // --- Book Copies ---
        public static Error BookCopyNotFound => Error.NotFound("BookCopy.NotFound", "The requested book copy was not found.");
        public static Error BarcodeInvalid => Error.Validation("BookCopy.InvalidBarcode", "The barcode must start with 'CPY-' followed by exactly 8 numeric digits.");
        public static Error LocationLength => Error.Validation("BookCopy.LocationTooLong", "The location string cannot exceed 100 characters.");
        public static Error BarcodeAlreadyExists => Error.Conflict("BookCopy.BarcodeConflict", "A book copy with this barcode already exists.");

        // --- Metadata (Genres, Keywords, Audiences, Authors, Categories, Publishers, Themes) ---
        public static Error GenreNameLength => Error.Validation("Genre.NameTooLong", "The genre name cannot exceed 50 characters.");
        public static Error GenreAlreadyExists => Error.Conflict("Genre.NameConflict", "A genre with this name already exists.");
        public static Error GenreNotFound => Error.NotFound("Genre.NotFound", "The requested genre was not found.");

        public static Error KeywordNameLength => Error.Validation("Keyword.NameTooLong", "The keyword name cannot exceed 50 characters.");
        public static Error KeywordAlreadyExists => Error.Conflict("Keyword.NameConflict", "A keyword with this name already exists.");
        public static Error KeywordNotFound => Error.NotFound("Keyword.NotFound", "The requested keyword was not found.");

        public static Error AudienceNameLength => Error.Validation("Audience.NameTooLong", "The audience name cannot exceed 50 characters.");
        public static Error AudienceAlreadyExists => Error.Conflict("Audience.NameConflict", "An audience with this name already exists.");
        public static Error AudienceNotFound => Error.NotFound("Audience.NotFound", "The requested audience was not found.");

        public static Error AuthorNameLength => Error.Validation("Author.NameTooLong", "The author name cannot exceed 50 characters.");
        public static Error AuthorAlreadyExists => Error.Conflict("Author.NameConflict", "An author with this name already exists.");
        public static Error AuthorNotFound => Error.NotFound("Author.NotFound", "The requested author was not found.");

        public static Error CategoryNameLength => Error.Validation("Category.NameTooLong", "The category name cannot exceed 50 characters.");
        public static Error CategoryAlreadyExists => Error.Conflict("Category.NameConflict", "A category with this name already exists.");
        public static Error CategoryNotFound => Error.NotFound("Category.NotFound", "The requested category was not found.");

        public static Error PublisherNameLength => Error.Validation("Publisher.NameTooLong", "The publisher name cannot exceed 50 characters.");
        public static Error PublisherAlreadyExists => Error.Conflict("Publisher.NameConflict", "A publisher with this name already exists.");
        public static Error PublisherNotFound => Error.NotFound("Publisher.NotFound", "The requested publisher was not found.");

        public static Error ThemeNameLength => Error.Validation("Theme.NameTooLong", "The theme name cannot exceed 50 characters.");
        public static Error ThemeAlreadyExists => Error.Conflict("Theme.NameConflict", "A theme with this name already exists.");
        public static Error ThemeNotFound => Error.NotFound("Theme.NotFound", "The requested theme was not found.");

        // --- Concurrency ---
        public static Error ConcurrencyConflict => Error.Conflict("System.ConcurrencyConflict", "The requested resource was modified by another process. Please try again.");

        // --- Users & Roles ---
        public static Error UserNotFound => Error.NotFound("User.NotFound", "The requested user was not found.");
        public static Error UserNotMember => Error.Forbidden("User.RequiresMemberRole", "This operation requires member privileges.");
        public static Error NotLibrarian => Error.Forbidden("User.RequiresLibrarianRole", "This operation requires librarian privileges.");

        public static Error EmailInvalid => Error.Validation("User.InvalidEmail", "The provided email address is invalid.");
        public static Error PhoneNumberInvalid => Error.Validation("User.InvalidPhoneNumber", "The provided phone number is invalid.");
        public static Error NameLength => Error.Validation("User.NameTooLong", "First and last names cannot exceed 50 characters.");
        public static Error AddressLength => Error.Validation("User.AddressTooLong", "The address cannot exceed 512 characters.");

        public static Error EmailIsUsed => Error.Conflict("User.EmailConflict", "This email address is already registered to another user.");
        public static Error PhoneNumberIsUsed => Error.Conflict("User.PhoneNumberConflict", "This phone number is already registered to another user.");
        public static Error UserHasUnpaidFines => Error.Validation("User.HasUnpaidFines", "This operation cannot proceed because the user has unpaid fines.");

        public static Error PasswordInvalid => Error.Validation("User.InvalidPasswordFormat", "The password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.");
        public static Error PasswordsDontMatch => Error.Validation("User.PasswordMismatch", "The provided current password does not match the actual password.");

        // --- Borrow Records & Circulation ---
        public static Error BorrowRecordNotFound => Error.NotFound("BorrowRecord.NotFound", "The requested borrow record was not found.");
        public static Error BorrowRecordStatusInvalid => Error.Conflict("BorrowRecord.InvalidStatusTransition", "The current status of this record does not allow the requested operation.");
        public static Error NewDueDateInvalid => Error.Validation("BorrowRecord.InvalidDueDate", "The new due date must be later than the current due date.");
        public static Error NotLate => Error.Conflict("BorrowRecord.NotOverdue", "This borrow record is not currently overdue.");
        public static Error AnotherCopyAlreadyBorrowed => Error.Conflict("BorrowRecord.DuplicateCopyBorrowed", "The user is already borrowing a copy of this book.");

        // --- Fines ---
        public static Error FineNotFound => Error.NotFound("Fine.NotFound", "The requested fine was not found.");
        public static Error FineDescriptionLength => Error.Validation("Fine.DescriptionTooLong", "The fine description cannot exceed 500 characters.");

        // --- Authentication & Tokens ---
        public static Error CredentialsInvalid => Error.Unauthorized("Auth.InvalidCredentials", "The provided email or password is incorrect.");
        public static Error ExpiredAccessTokenInvalid => Error.Unauthorized("Auth.InvalidExpiredToken", "The provided expired access token is invalid or malformed.");
        public static Error UserIdClaimInvalid => Error.Unauthorized("Auth.InvalidUserIdClaim", "The user identifier claim in the token is invalid or missing.");
        public static Error UserIdInvalid => Error.Validation("Auth.InvalidUserId", "The provided user identifier is invalid.");
        public static Error RefreshTokenExpired => Error.Unauthorized("Auth.RefreshTokenExpired", "The refresh token has expired. Please log in again.");
    }
}
