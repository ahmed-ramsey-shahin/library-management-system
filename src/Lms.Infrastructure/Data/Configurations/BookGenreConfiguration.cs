using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
    {
        public void Configure(EntityTypeBuilder<BookGenre> builder)
        {
            builder.ToTable("book_genres");
            builder.HasKey(entity => new { entity.GenreId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.GenreId).HasColumnName("genre_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookGenres)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Genre)
                .WithMany()
                .HasForeignKey(entity => entity.GenreId);
        }
    }
}
