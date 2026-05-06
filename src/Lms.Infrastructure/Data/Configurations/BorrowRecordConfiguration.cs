using Lms.Domain.Circulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
    {
        public void Configure(EntityTypeBuilder<BorrowRecord> builder)
        {
            builder.ToTable("borrow_records");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.CreatedAt)
                .HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted)
                .HasColumnName("is_deleted");
            builder.Property(entity => entity.Id)
                .HasColumnName("id");
            builder.Property(entity => entity.MemberId)
                .HasColumnName("member_id");
            builder.Property(entity => entity.BookCopyId)
                .HasColumnName("book_copy_id");
            builder.Property(entity => entity.DueDate)
                .HasColumnName("due_date");
            builder.Property(entity => entity.BorrowingCost)
                .HasColumnName("borrowing_cost");
            builder.Property(entity => entity.RenewalCount)
                .HasColumnName("renewal_count");
            builder.Property(entity => entity.PickupDeadline)
                .HasColumnName("pickup_deadline");
            builder.Property(entity => entity.PickedUp)
                .HasColumnName("pickedup");
            builder.Property(entity => entity.Status)
                .HasColumnName("status");

            builder.HasOne(entity => entity.Member)
                .WithMany(member => member.BorrowRecords)
                .HasForeignKey(entity => entity.MemberId);

            builder.HasOne(entity => entity.BookCopy)
                .WithMany()
                .HasForeignKey(entity => entity.BookCopyId);

            builder.HasMany(entity => entity.Fines)
                .WithOne(fine => fine.BorrowRecord)
                .HasForeignKey(fine => fine.BorrowRecordId);
        }
    }
}
