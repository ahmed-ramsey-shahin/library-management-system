using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.ToTable("book_authors");
            builder.HasKey(entity => new { entity.AuthorId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.AuthorId).HasColumnName("author_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookAuthors)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Author)
                .WithMany()
                .HasForeignKey(entity => entity.AuthorId);
            builder.HasQueryFilter(entity => !entity.Book.IsDeleted);
        }
    }
}
