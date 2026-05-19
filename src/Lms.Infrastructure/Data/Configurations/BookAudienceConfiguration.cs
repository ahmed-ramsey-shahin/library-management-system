using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookAudienceConfiguration : IEntityTypeConfiguration<BookAudience>
    {
        public void Configure(EntityTypeBuilder<BookAudience> builder)
        {
            builder.ToTable("book_audiences");
            builder.HasKey(entity => new { entity.AudienceId, entity.BookId });
            builder.Property(entity => entity.BookId).HasColumnName("book_id");
            builder.Property(entity => entity.AudienceId).HasColumnName("audience_id");
            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookAudiences)
                .HasForeignKey(entity => entity.BookId);
            builder.HasOne(entity => entity.Audience)
                .WithMany()
                .HasForeignKey(entity => entity.AudienceId);
            builder.HasQueryFilter(entity => !entity.Book.IsDeleted);
        }
    }
}
