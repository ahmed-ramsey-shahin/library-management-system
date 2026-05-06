using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookThemeConfiguration : IEntityTypeConfiguration<BookTheme>
    {
        public void Configure(EntityTypeBuilder<BookTheme> builder)
        {
            builder.ToTable("book_themes");
            builder.HasKey(entity => new { entity.ThemeId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.ThemeId).HasColumnName("theme_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookThemes)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Theme)
                .WithMany()
                .HasForeignKey(entity => entity.ThemeId);
        }
    }
}
