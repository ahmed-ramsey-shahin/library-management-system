using Lms.Domain.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("genres");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.Id).HasColumnName("id");
            builder.Property(entity => entity.Name).HasColumnName("name");
            builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted).HasColumnName("is_deleted");
        }
    }
}
