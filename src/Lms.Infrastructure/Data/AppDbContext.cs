using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Circulation;
using Lms.Domain.Common;
using Lms.Domain.Identity;
using Lms.Domain.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : DbContext(options), IAppDbContext
    {
        public DbSet<Book> Books => Set<Book>();

        public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();

        public DbSet<Author> Authors => Set<Author>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Keyword> Keywords => Set<Keyword>();

        public DbSet<Audience> Audiences => Set<Audience>();

        public DbSet<Theme> Themes => Set<Theme>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<BookCopy> BookCopies => Set<BookCopy>();

        public DbSet<Fine> Fines => Set<Fine>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Publisher> Publishers => Set<Publisher>();

        public DbSet<LibrarianCategory> LibrarianCategories => Set<LibrarianCategory>();

        public DbSet<BookCategory> BookCategories => Set<BookCategory>();

        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();

        public DbSet<BookKeyword> BookKeywords => Set<BookKeyword>();

        public DbSet<BookAudience> BookAudiences => Set<BookAudience>();

        public DbSet<BookTheme> BookThemes => Set<BookTheme>();

        public DbSet<BookGenre> BookGenres => Set<BookGenre>();

        public DbSet<User> Users => Set<User>();

        public void SetOriginalVersion<TEntity>(TEntity entity, byte[] version) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private async Task DispatchDomainEvents(CancellationToken cancellationToken)
        {
            var eventfulEntities = ChangeTracker.Entries()
                .Where(e => e.Entity is EventfulEntity baseEntity && baseEntity.Events.Count != 0)
                .Select(e => (EventfulEntity) e.Entity)
                .ToList();
            var domainEvents = eventfulEntities
                .SelectMany(e => e.Events)
                .ToList();

            foreach (var @event in domainEvents)
            {
                await mediator.Publish(@event, cancellationToken);
            }

            foreach (var entity in eventfulEntities)
            {
                entity.ClearEvents();
            }
        }
    }
}
