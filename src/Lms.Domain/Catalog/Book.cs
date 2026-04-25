using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class Book : AuditableEntity
    {
        public Guid Id { get; }
        public string Isbn { get; private set; } = string.Empty;
        public string Issn { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public int PageCount { get; private set; }
        public Guid PublisherId { get; private set; }
        public DateOnly PublishingDate { get; private set; }
        public string Language { get; private set; } = string.Empty;
        public string Edition { get; private set; } = string.Empty;
        public decimal BorrowPricePerDay { get; private set; }
        public decimal FinePerDay { get; private set; }
        public decimal LostFee { get; private set; }
        public decimal DamageFee { get; private set; }
        private readonly List<BookCopy> _bookCopies = [];
        public IReadOnlyCollection<BookCopy> BookCopies => _bookCopies.AsReadOnly();
        private readonly List<BookCategory> _bookCategories = [];
        public IReadOnlyCollection<BookCategory> BookCategories => _bookCategories.AsReadOnly();
        private readonly List<BookKeyword> _bookKeywords = [];
        public IReadOnlyCollection<BookKeyword> BookKeywords => _bookKeywords.AsReadOnly();
        private readonly List<BookTheme> _bookThemes = [];
        public IReadOnlyCollection<BookTheme> BookThemes => _bookThemes.AsReadOnly();
        private readonly List<BookGenre> _bookGenres = [];
        public IReadOnlyCollection<BookGenre> BookGenres => _bookGenres.AsReadOnly();
        private readonly List<BookAudience> _bookAudiences = [];
        public IReadOnlyCollection<BookAudience> BookAudiences => _bookAudiences.AsReadOnly();
        private readonly List<BookAuthor> _bookAuthors = [];
        public IReadOnlyCollection<BookAuthor> BookAuthors => _bookAuthors.AsReadOnly();
        public int AvailableCopies => _bookCopies.Count(copy => copy.State == BookCopyState.Available);

        private Book()
        {}

        private Book(
            Guid id,
            string isbn,
            string issn,
            string title,
            string? description,
            int pageCount,
            Guid publisherId,
            DateOnly publishingDate,
            string language,
            string edition,
            decimal borrowPricePerDay,
            decimal finePerDay,
            decimal lostFee,
            decimal damageFee
        )
        {
            Id = id;
            Isbn = isbn;
            Issn = issn;
            Title = title;
            Description = description;
            PageCount = pageCount;
            PublisherId = publisherId;
            PublishingDate = publishingDate;
            Language = language;
            Edition = edition;
            BorrowPricePerDay = borrowPricePerDay;
            FinePerDay = finePerDay;
            LostFee = lostFee;
            DamageFee = damageFee;
        }

        public static Result<Book> Create(
            Guid id,
            string isbn,
            string issn,
            string title,
            string? description,
            int pageCount,
            Guid publisherId,
            DateOnly publishingDate,
            string edition,
            decimal borrowPricePerDay,
            decimal finePerDay,
            decimal lostFee,
            decimal damageFee,
            string language="english"
        )
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(BookErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(isbn))
            {
                errors.Add(BookErrors.IsbnRequired);
            }

            if (string.IsNullOrWhiteSpace(issn))
            {
                errors.Add(BookErrors.IssnRequired);
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                errors.Add(BookErrors.TitleRequired);
            }

            if (publisherId == Guid.Empty)
            {
                errors.Add(BookErrors.PublisherIdRequired);
            }

            if (publishingDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                errors.Add(BookErrors.InvalidPublishingDate);
            }

            if (string.IsNullOrWhiteSpace(edition))
            {
                errors.Add(BookErrors.EditionRequired);
            }

            if (borrowPricePerDay <= 0)
            {
                errors.Add(BookErrors.BorrowPricePerDayInvalid);
            }

            if (finePerDay <= 0)
            {
                errors.Add(BookErrors.FinePerDayInvalid);
            }

            if (lostFee <= 0)
            {
                errors.Add(BookErrors.LostFeeInvalid);
            }

            if (damageFee <= 0)
            {
                errors.Add(BookErrors.DamageFeeInvalid);
            }

            if (string.IsNullOrWhiteSpace(language))
            {
                errors.Add(BookErrors.LanguageRequired);
            }

            if (pageCount <= 0)
            {
                errors.Add(BookErrors.PageCountInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Book(
                id,
                isbn,
                issn,
                title,
                description,
                pageCount,
                publisherId,
                publishingDate,
                language,
                edition,
                borrowPricePerDay,
                finePerDay,
                lostFee,
                damageFee
            );
        }

        public Result<Deleted> Delete(int associatedBooks)
        {
            if (associatedBooks > 0)
            {
                return BookErrors.BookHasCopies;
            }

            IsDeleted = true;
            return Result.Deleted;
        }

        public Result<Updated> UpdateDetails(
            string isbn,
            string issn,
            string title,
            string? description,
            int pageCount,
            Guid publisherId,
            DateOnly publishingDate,
            string edition,
            string language="english"
        )
        {
            List<Error> errors = [];

            if (string.IsNullOrWhiteSpace(isbn))
            {
                errors.Add(BookErrors.IsbnRequired);
            }

            if (string.IsNullOrWhiteSpace(issn))
            {
                errors.Add(BookErrors.IssnRequired);
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                errors.Add(BookErrors.TitleRequired);
            }

            if (publisherId == Guid.Empty)
            {
                errors.Add(BookErrors.PublisherIdRequired);
            }

            if (publishingDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                errors.Add(BookErrors.InvalidPublishingDate);
            }

            if (string.IsNullOrWhiteSpace(edition))
            {
                errors.Add(BookErrors.EditionRequired);
            }

            if (string.IsNullOrWhiteSpace(language))
            {
                errors.Add(BookErrors.LanguageRequired);
            }

            if (pageCount <= 0)
            {
                errors.Add(BookErrors.PageCountInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            Isbn = isbn;
            Issn = issn;
            Title = title;
            Description = description;
            PageCount = pageCount;
            PublisherId = publisherId;
            PublishingDate = publishingDate;
            Language = language;
            Edition = edition;
            return Result.Updated;
        }

        public Result<Updated> UpdateFinancials(
            decimal borrowPricePerDay,
            decimal finePerDay,
            decimal lostFee,
            decimal damageFee
        )
        {
            List<Error> errors = [];

            if (borrowPricePerDay <= 0)
            {
                errors.Add(BookErrors.BorrowPricePerDayInvalid);
            }

            if (finePerDay <= 0)
            {
                errors.Add(BookErrors.FinePerDayInvalid);
            }

            if (lostFee <= 0)
            {
                errors.Add(BookErrors.LostFeeInvalid);
            }

            if (damageFee <= 0)
            {
                errors.Add(BookErrors.DamageFeeInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            BorrowPricePerDay = borrowPricePerDay;
            FinePerDay = finePerDay;
            LostFee = lostFee;
            DamageFee = damageFee;
            return Result.Updated;
        }

        public Result<Updated> AddCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return BookErrors.CategoryIdRequired;
            }

            if (_bookCategories.Any(category => category.CategoryId == categoryId))
            {
                return BookErrors.CategoryAlreadyAssigned;
            }

            _bookCategories.Add(new BookCategory(Id, categoryId));
            return Result.Updated;
        }

        public Result<Updated> RemoveCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return BookErrors.CategoryIdRequired;
            }

            if (!_bookCategories.Any(category => category.CategoryId == categoryId))
            {
                return BookErrors.CategoryNotAssigned;
            }

            _bookCategories.RemoveAll(c => c.CategoryId == categoryId);
            return Result.Updated;
        }

        public Result<Updated> AddAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                return BookErrors.AuthorIdRequired;
            }

            if (_bookAuthors.Any(a => a.AuthorId == authorId))
            {
                return BookErrors.AuthorAlreadyAssigned;
            }

            _bookAuthors.Add(new BookAuthor(Id, authorId));
            return Result.Updated;
        }

        public Result<Updated> RemoveAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                return BookErrors.AuthorIdRequired;
            }

            if (!_bookAuthors.Any(a => a.AuthorId == authorId))
            {
                return BookErrors.AuthorNotAssigned;
            }

            _bookAuthors.RemoveAll(a => a.AuthorId == authorId);
            return Result.Updated;
        }

        public Result<Updated> AddKeyword(Guid keywordId)
        {
            if (keywordId == Guid.Empty)
            {
                return BookErrors.KeywordIdRequired;
            }

            if (_bookKeywords.Any(a => a.KeywordId == keywordId))
            {
                return BookErrors.KeywordAlreadyAssigned;
            }

            _bookKeywords.Add(new BookKeyword(Id, keywordId));
            return Result.Updated;
        }

        public Result<Updated> RemoveKeyword(Guid keywordId)
        {
            if (keywordId == Guid.Empty)
            {
                return BookErrors.KeywordIdRequired;
            }

            if (!_bookKeywords.Any(a => a.KeywordId == keywordId))
            {
                return BookErrors.KeywordNotAssigned;
            }

            _bookKeywords.RemoveAll(a => a.KeywordId == keywordId);
            return Result.Updated;
        }

        public Result<Updated> AddAudience(Guid audienceId)
        {
            if (audienceId == Guid.Empty)
            {
                return BookErrors.AudienceIdRequired;
            }

            if (_bookAudiences.Any(a => a.AudienceId == audienceId))
            {
                return BookErrors.AudienceAlreadyAssigned;
            }

            _bookAudiences.Add(new BookAudience(Id, audienceId));
            return Result.Updated;
        }

        public Result<Updated> RemoveAudience(Guid audienceId)
        {
            if (audienceId == Guid.Empty)
            {
                return BookErrors.AudienceIdRequired;
            }

            if (!_bookAudiences.Any(a => a.AudienceId == audienceId))
            {
                return BookErrors.AudienceNotAssigned;
            }

            _bookAudiences.RemoveAll(a => a.AudienceId == audienceId);
            return Result.Updated;
        }

        public Result<Updated> AddTheme(Guid themeId)
        {
            if (themeId == Guid.Empty)
            {
                return BookErrors.ThemeIdRequired;
            }

            if (_bookThemes.Any(a => a.ThemeId == themeId))
            {
                return BookErrors.ThemeAlreadyAssigned;
            }

            _bookThemes.Add(new BookTheme(Id, themeId));
            return Result.Updated;
        }

        public Result<Updated> RemoveTheme(Guid themeId)
        {
            if (themeId == Guid.Empty)
            {
                return BookErrors.ThemeIdRequired;
            }

            if (!_bookThemes.Any(a => a.ThemeId == themeId))
            {
                return BookErrors.ThemeNotAssigned;
            }

            _bookThemes.RemoveAll(a => a.ThemeId == themeId);
            return Result.Updated;
        }

        public Result<Updated> AddGenre(Guid genreId)
        {
            if (genreId == Guid.Empty)
            {
                return BookErrors.GenreIdRequired;
            }

            if (_bookGenres.Any(a => a.GenreId == genreId))
            {
                return BookErrors.GenreAlreadyAssigned;
            }

            _bookGenres.Add(new BookGenre(Id, genreId));
            return Result.Updated;
        }

        public Result<Updated> RemoveGenre(Guid genreId)
        {
            if (genreId == Guid.Empty)
            {
                return BookErrors.GenreIdRequired;
            }

            if (!_bookGenres.Any(a => a.GenreId == genreId))
            {
                return BookErrors.GenreNotAssigned;
            }

            _bookGenres.RemoveAll(a => a.GenreId == genreId);
            return Result.Updated;
        }

        public Result<BookCopy> AddCopy(
            Guid id,
            string barcode,
            string location,
            DateOnly? acquisitionDate,
            BookCopyStatus status=BookCopyStatus.Good,
            BookCopyState state=BookCopyState.Available
        )
        {
            var copyResult = BookCopy.Create(id, Id, barcode, location, acquisitionDate, status, state);

            if (copyResult.IsError)
            {
                return copyResult.Errors!;
            }

            _bookCopies.Add(copyResult.Value);
            return copyResult.Value;
        }

        public Result<Updated> RemoveCopy(Guid copyId)
        {
            if (copyId == Guid.Empty)
            {
                return BookCopyErrors.IdRequired;
            }

            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            var result = copy.Delete();

            if (result.IsSuccess)
            {
                return Result.Updated;
            }
            else
            {
                return result.Errors!;
            }
        }

        public Result<BookCopy> AllocateAvailableCopy()
        {
            var copy = _bookCopies.FirstOrDefault(c => c.State == BookCopyState.Available);

            if (copy is null)
            {
                return BookErrors.NoAvailableCopies;
            }

            var result = copy.MarkAsWaitingApproval();

            if (result.IsError)
            {
                return result.Errors!;
            }

            return copy;
        }
    }
}
