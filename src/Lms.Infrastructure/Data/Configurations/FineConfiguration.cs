using Lms.Domain.Circulation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class FineConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.ToTable("fines");
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
            builder.Property(entity => entity.BorrowRecordId)
                .HasColumnName("borrow_record_id");
            builder.Property(entity => entity.Status)
                .HasColumnName("status");
            builder.Property(entity => entity.Amount)
                .HasColumnName("amount");
            builder.Property(entity => entity.Description)
                .HasColumnName("description");
            builder.Property(entity => entity.FineDate)
                .HasColumnName("fine_date");
            builder.Property(entity => entity.PaidAt)
                .HasColumnName("paid_at");

            builder.HasOne(entity => entity.Member)
                .WithMany(member => member.Fines)
                .HasForeignKey(entity => entity.MemberId);

            builder.HasOne(entity => entity.BorrowRecord)
                .WithMany(borrowRecord => borrowRecord.Fines)
                .HasForeignKey(entity => entity.BorrowRecordId);
        }
    }
}
