using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("authors");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.Id).HasColumnName("id");
            builder.Property(entity => entity.Name).HasColumnName("name");
            builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted).HasColumnName("is_deleted");
        }
    }
}
