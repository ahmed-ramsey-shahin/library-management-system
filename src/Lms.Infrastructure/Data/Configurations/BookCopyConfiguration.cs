using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            builder.ToTable("book_copies");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.CreatedAt)
                .HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted)
                .HasColumnName("is_deleted");
            builder.Property(entity => entity.Id)
                .HasColumnName("id");
            builder.Property(entity => entity.BookId)
                .HasColumnName("book_id");
            builder.Property(entity => entity.Barcode)
                .HasColumnName("barcode");
            builder.Property(entity => entity.Status)
                .HasColumnName("status");
            builder.Property(entity => entity.State)
                .HasColumnName("state");
            builder.Property(entity => entity.Location)
                .HasColumnName("location");
            builder.Property(entity => entity.Version)
                .HasColumnName("version")
                .IsRowVersion();
            builder.Property(entity => entity.AcquisitionDate)
                .HasColumnName("acquisition_date");

            builder.HasOne(entity => entity.Book)
                .WithMany(book => book.BookCopies)
                .HasForeignKey(entity => entity.BookId);
        }
    }
}
