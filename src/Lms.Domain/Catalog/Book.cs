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

        public Result<Updated> UpsertAudiences(IEnumerable<Guid> audienceIds)
        {
            var hasEmptyId = audienceIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.AudienceIdRequired;
            }

            audienceIds = [.. audienceIds.Distinct()];
            List<Guid> currentAudiences = [.. _bookAudiences.Select(bookAudience => bookAudience.AudienceId)];
            List<Guid> audiencesToAdd = [.. audienceIds.Except(currentAudiences)];
            List<Guid> audiencesToRemove = [.. currentAudiences.Except(audienceIds)];

            if (audiencesToRemove.Count > 0)
            {
                _bookAudiences.RemoveAll(bookAudience => audiencesToRemove.Contains(bookAudience.AudienceId));
            }

            foreach (var audienceId in audiencesToAdd)
            {
                _bookAudiences.Add(new BookAudience(Id, audienceId));
            }

            return Result.Updated;
        }

        public Result<Updated> UpsertAuthors(IEnumerable<Guid> authorIds)
        {
            var hasEmptyId = authorIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.AuthorIdRequired;
            }

            authorIds = [.. authorIds.Distinct()];
            List<Guid> currentAuthors = [.. _bookAuthors.Select(bookAuthor => bookAuthor.AuthorId)];
            List<Guid> authorsToAdd = [.. authorIds.Except(currentAuthors)];
            List<Guid> authorsToRemove = [.. currentAuthors.Except(authorIds)];

            if (authorsToRemove.Count > 0)
            {
                _bookAuthors.RemoveAll(bookAuthor => authorsToRemove.Contains(bookAuthor.AuthorId));
            }

            foreach (var authorId in authorsToAdd)
            {
                _bookAuthors.Add(new BookAuthor(Id, authorId));
            }

            return Result.Updated;
        }

        public Result<Updated> UpsertCategories(IEnumerable<Guid> categoryIds)
        {
            var hasEmptyId = categoryIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.CategoryIdRequired;
            }

            categoryIds = [.. categoryIds.Distinct()];
            List<Guid> currentCategories = [.. _bookCategories.Select(bookCategory => bookCategory.CategoryId)];
            List<Guid> categoriesToAdd = [.. categoryIds.Except(currentCategories)];
            List<Guid> categoriesToRemove = [.. currentCategories.Except(categoryIds)];

            if (categoriesToRemove.Count > 0)
            {
                _bookCategories.RemoveAll(bookCategory => categoriesToRemove.Contains(bookCategory.CategoryId));
            }

            foreach (var categoryId in categoriesToAdd)
            {
                _bookCategories.Add(new BookCategory(Id, categoryId));
            }

            return Result.Updated;
        }

        public Result<Updated> UpsertGenres(IEnumerable<Guid> genreIds)
        {
            var hasEmptyId = genreIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.GenreIdRequired;
            }

            genreIds = [.. genreIds.Distinct()];
            List<Guid> currentGenres = [.. _bookGenres.Select(bookGenre => bookGenre.GenreId)];
            List<Guid> genresToAdd = [.. genreIds.Except(currentGenres)];
            List<Guid> genresToRemove = [.. currentGenres.Except(genreIds)];

            if (genresToRemove.Count > 0)
            {
                _bookGenres.RemoveAll(bookGenre => genresToRemove.Contains(bookGenre.GenreId));
            }

            foreach (var genreId in genresToAdd)
            {
                _bookGenres.Add(new BookGenre(Id, genreId));
            }

            return Result.Updated;
        }

        public Result<Updated> UpsertKeywords(IEnumerable<Guid> keywordIds)
        {
            var hasEmptyId = keywordIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.KeywordIdRequired;
            }

            keywordIds = [.. keywordIds.Distinct()];
            List<Guid> currentKeywords = [.. _bookKeywords.Select(bookKeyword => bookKeyword.KeywordId)];
            List<Guid> keywordsToAdd = [.. keywordIds.Except(currentKeywords)];
            List<Guid> keywordsToRemove = [.. currentKeywords.Except(keywordIds)];

            if (keywordsToRemove.Count > 0)
            {
                _bookKeywords.RemoveAll(bookKeyword => keywordsToRemove.Contains(bookKeyword.KeywordId));
            }

            foreach (var keywordId in keywordsToAdd)
            {
                _bookKeywords.Add(new BookKeyword(Id, keywordId));
            }

            return Result.Updated;
        }

        public Result<Updated> UpsertThemes(IEnumerable<Guid> themeIds)
        {
            var hasEmptyId = themeIds.Contains(Guid.Empty);

            if (hasEmptyId)
            {
                return BookErrors.ThemeIdRequired;
            }

            themeIds = [.. themeIds.Distinct()];
            List<Guid> currentThemes = [.. _bookThemes.Select(bookTheme => bookTheme.ThemeId)];
            List<Guid> themesToAdd = [.. themeIds.Except(currentThemes)];
            List<Guid> themesToRemove = [.. currentThemes.Except(themeIds)];

            if (themesToRemove.Count > 0)
            {
                _bookThemes.RemoveAll(bookTheme => themesToRemove.Contains(bookTheme.ThemeId));
            }

            foreach (var themeId in themesToAdd)
            {
                _bookThemes.Add(new BookTheme(Id, themeId));
            }

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

        public Result<Updated> ChangeCopyStatus(Guid copyId, BookCopyStatus status)
        {
            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            return copy.ChangeStatus(status);
        }

        public Result<Updated> MarkCopyAsBorrowed(Guid copyId)
        {
            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            return copy.MarkAsBorrowed();
        }

        public Result<Updated> MarkCopyAsAvailable(Guid copyId)
        {
            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            return copy.MarkAsAvailable();
        }

        public Result<Updated> MarkCopyAsMaintenance(Guid copyId)
        {
            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            return copy.MarkAsMaintenance();
        }

        public Result<Updated> ChangeCopyLocation(Guid copyId, string location)
        {
            var copy = _bookCopies.FirstOrDefault(copy => copy.Id == copyId);

            if (copy is null)
            {
                return BookErrors.CopyNotFound;
            }

            return copy.ChangeLocation(location);
        }
    }
}
