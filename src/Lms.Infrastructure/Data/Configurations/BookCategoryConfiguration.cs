using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookCategoryConfiguration : IEntityTypeConfiguration<BookCategory>
    {
        public void Configure(EntityTypeBuilder<BookCategory> builder)
        {
            builder.ToTable("book_categories");
            builder.HasKey(entity => new { entity.CategoryId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.CategoryId).HasColumnName("category_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookCategories)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Category)
                .WithMany()
                .HasForeignKey(entity => entity.CategoryId);
            builder.HasQueryFilter(entity => !entity.Book.IsDeleted);
        }
    }
}
