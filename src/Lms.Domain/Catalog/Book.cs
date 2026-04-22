using Lms.Domain.Common;
using Lms.Domain.Common.Results;
using Lms.Domain.Metadata;

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
        public Publisher Publisher { get; private set; } = null!;
        public DateOnly PublishingDate { get; private set; }
        public string Language { get; private set; } = string.Empty;
        public string Edition { get; private set; } = string.Empty;
        public decimal BorrowPricePerDay { get; private set; }
        public decimal FinePerDay { get; private set; }
        public decimal LostFee { get; private set; }
        public decimal DamageFee { get; private set; }
        private readonly List<BookCopy> _bookCopies = [];
        public IReadOnlyCollection<BookCopy> BookCopies => _bookCopies.AsReadOnly();
        private readonly List<Category> _categories = [];
        public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
        private readonly List<Author> _authors = [];
        public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
        private readonly List<Keyword> _keywords = [];
        public IReadOnlyCollection<Keyword> Keywords => _keywords.AsReadOnly();
        private readonly List<Audience> _audiences = [];
        public IReadOnlyCollection<Audience> Audiences => _audiences.AsReadOnly();
        private readonly List<Theme> _themes = [];
        public IReadOnlyCollection<Theme> Themes => _themes.AsReadOnly();
        private readonly List<Genre> _genres = [];
        public IReadOnlyCollection<Genre> Genres => _genres.AsReadOnly();

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

        public Result<Updated> AddCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);
            if (_categories.Any(c => c.Id == category.Id))
            {
                return BookErrors.CategoryAlreadyAssigned;
            }
            _categories.Add(category);
            return Result.Updated;
        }

        public Result<Updated> RemoveCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);
            if (!_categories.Any(c => c.Id == category.Id))
            {
                return BookErrors.CategoryNotAssigned;
            }
            _categories.RemoveAll(c => c.Id == category.Id);
            return Result.Updated;
        }

        public Result<Updated> AddAuthor(Author author)
        {
            ArgumentNullException.ThrowIfNull(author);
            if (_authors.Any(a => a.Id == author.Id))
            {
                return BookErrors.AuthorAlreadyAssigned;
            }
            _authors.Add(author);
            return Result.Updated;
        }

        public Result<Updated> RemoveAuthor(Author author)
        {
            ArgumentNullException.ThrowIfNull(author);
            if (!_authors.Any(a => a.Id == author.Id))
            {
                return BookErrors.AuthorNotAssigned;
            }
            _authors.RemoveAll(a => a.Id == author.Id);
            return Result.Updated;
        }

        public Result<Updated> AddKeyword(Keyword keyword)
        {
            ArgumentNullException.ThrowIfNull(keyword);
            if (_keywords.Any(k => k.Id == keyword.Id))
            {
                return BookErrors.KeywordAlreadyAssigned;
            }
            _keywords.Add(keyword);
            return Result.Updated;
        }

        public Result<Updated> RemoveKeyword(Keyword keyword)
        {
            ArgumentNullException.ThrowIfNull(keyword);
            if (!_keywords.Any(k => k.Id == keyword.Id))
            {
                return BookErrors.KeywordNotAssigned;
            }
            _keywords.RemoveAll(k => k.Id == keyword.Id);
            return Result.Updated;
        }

        public Result<Updated> AddAudience(Audience audience)
        {
            ArgumentNullException.ThrowIfNull(audience);
            if (_audiences.Any(a => a.Id == audience.Id))
            {
                return BookErrors.AudienceAlreadyAssigned;
            }
            _audiences.Add(audience);
            return Result.Updated;
        }

        public Result<Updated> RemoveAudience(Audience audience)
        {
            ArgumentNullException.ThrowIfNull(audience);
            if (!_audiences.Any(a => a.Id == audience.Id))
            {
                return BookErrors.AudienceNotAssigned;
            }
            _audiences.RemoveAll(a => a.Id == audience.Id);
            return Result.Updated;
        }

        public Result<Updated> AddTheme(Theme theme)
        {
            ArgumentNullException.ThrowIfNull(theme);
            if (_themes.Any(t => t.Id == theme.Id))
            {
                return BookErrors.ThemeAlreadyAssigned;
            }
            _themes.Add(theme);
            return Result.Updated;
        }

        public Result<Updated> RemoveTheme(Theme theme)
        {
            ArgumentNullException.ThrowIfNull(theme);
            if (!_themes.Any(t => t.Id == theme.Id))
            {
                return BookErrors.ThemeNotAssigned;
            }
            _themes.RemoveAll(t => t.Id == theme.Id);
            return Result.Updated;
        }

        public Result<Updated> AddGenre(Genre genre)
        {
            ArgumentNullException.ThrowIfNull(genre);
            if (_genres.Any(g => g.Id == genre.Id))
            {
                return BookErrors.GenreAlreadyAssigned;
            }
            _genres.Add(genre);
            return Result.Updated;
        }

        public Result<Updated> RemoveGenre(Genre genre)
        {
            ArgumentNullException.ThrowIfNull(genre);
            if (!_genres.Any(g => g.Id == genre.Id))
            {
                return BookErrors.GenreNotAssigned;
            }
            _genres.RemoveAll(g => g.Id == genre.Id);
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
            return BookCopy.Create(id, Id, barcode, location, acquisitionDate, status, state);
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

            copy.MarkAsBorrowed();
            return copy;
        }
    }
}
