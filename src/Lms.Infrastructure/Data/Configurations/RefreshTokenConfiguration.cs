using Lms.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Id)
                .HasColumnName("id");
            builder.Property(entity => entity.Token)
                .HasColumnName("token");
            builder.Property(entity => entity.UserId)
                .HasColumnName("user_id");
            builder.Property(entity => entity.ExpiresOn)
                .HasColumnName("expires_on");
            builder.Property(entity => entity.IsRevoked)
                .HasColumnName("is_revoked");
            builder.Property(entity => entity.RevokedAt)
                .HasColumnName("revoked_at");
        }
    }
}
