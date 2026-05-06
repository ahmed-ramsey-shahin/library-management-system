using Lms.Domain.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
    {
        public void Configure(EntityTypeBuilder<Keyword> builder)
        {
            builder.ToTable("keywoprds");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.Id).HasColumnName("id");
            builder.Property(entity => entity.Name).HasColumnName("name");
            builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted).HasColumnName("is_deleted");
        }
    }
}
