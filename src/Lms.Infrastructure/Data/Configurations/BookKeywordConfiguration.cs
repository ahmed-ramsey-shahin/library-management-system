using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookKeywordConfiguration : IEntityTypeConfiguration<BookKeyword>
    {
        public void Configure(EntityTypeBuilder<BookKeyword> builder)
        {
            builder.ToTable("book_keywords");
            builder.HasKey(entity => new { entity.KeywordId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.KeywordId).HasColumnName("keyword_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookKeywords)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Keyword)
                .WithMany()
                .HasForeignKey(entity => entity.KeywordId);
        }
    }
}
