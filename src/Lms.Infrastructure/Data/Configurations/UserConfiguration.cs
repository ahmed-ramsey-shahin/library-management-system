using Lms.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.CreatedAt)
                .HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted)
                .HasColumnName("is_deleted");
            builder.Property(entity => entity.Id)
                .HasColumnName("id");
            builder.Property(entity => entity.Email)
                .HasColumnName("email");
            builder.Property(entity => entity.FirstName)
                .HasColumnName("first_name");
            builder.Property(entity => entity.LastName)
                .HasColumnName("last_name");
            builder.Property(entity => entity.PhoneNumber)
                .HasColumnName("phone_number");
            builder.Property(entity => entity.Address)
                .HasColumnName("address");
            builder.Property(entity => entity.LibraryCardNumber)
                .HasColumnName("library_card_number");
            builder.Property(entity => entity.Role)
                .HasColumnName("role");
            builder.Property(entity => entity.Status)
                .HasColumnName("status");
            builder.Property(entity => entity.Password)
                .HasColumnName("password");
            builder.HasMany(entity => entity.LibrarianCategories)
                .WithOne(librarianCategory => librarianCategory.Librarian)
                .HasForeignKey(librarianCategory => librarianCategory.UserId);
            builder.HasMany(entity => entity.RefreshTokens)
                .WithOne()
                .HasForeignKey(refreshToken => refreshToken.UserId);
            builder.HasMany(entity => entity.BorrowRecords)
                .WithOne(borrowRecord => borrowRecord.Member)
                .HasForeignKey(borrowRecord => borrowRecord.MemberId);
            builder.HasMany(entity => entity.Fines)
                .WithOne(fine => fine.Member)
                .HasForeignKey(fine => fine.MemberId);
        }
    }
}
