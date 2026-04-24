using Lms.Domain.Catalog;
using Lms.Domain.Circulation;
using Lms.Domain.Identity;
using Lms.Domain.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Book> Books { get; }
        public DbSet<BorrowRecord> BorrowRecords { get; }
        public DbSet<Author> Authors { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Keyword> Keywords { get; }
        public DbSet<Audience> Audiencies { get; }
        public DbSet<Theme> Themes { get; }
        public DbSet<Genre> Genres { get; }
        public DbSet<BookCopy> BookCopies { get; }
        public DbSet<Fine> Fines { get; }
        public DbSet<RefreshToken> RefreshTokens { get; }
        public DbSet<Publisher> Publishers { get; }
        public DbSet<LibrarianCategory> LibrarianCategories { get; }
        public DbSet<BookCategory> BookCategories { get; }
        public DbSet<BookAuthor> BookAuthors { get; }
        public DbSet<BookKeyword> BookKeywords { get; }
        public DbSet<BookAudience> BookAudiences { get; }
        public DbSet<BookTheme> BookThemes { get; }
        public DbSet<BookGenre> BookGenres { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
