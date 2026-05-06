using Lms.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class LibrarianCategoryConfiguration : IEntityTypeConfiguration<LibrarianCategory>
    {
        public void Configure(EntityTypeBuilder<LibrarianCategory> builder)
        {
            builder.ToTable("librarian_categories");
            builder.HasKey(entity => new { entity.CategoryId, entity.UserId });
            builder.Property(entity => entity.UserId)
                .HasColumnName("user_id");
            builder.Property(entity => entity.CategoryId)
                .HasColumnName("category_id");
            builder.HasOne(entity => entity.Librarian)
                .WithMany(librarian => librarian.LibrarianCategories)
                .HasForeignKey(entity => entity.UserId);
            builder.HasOne(entity => entity.Category)
                .WithMany()
                .HasForeignKey(entity => entity.CategoryId);
        }
    }
}
