using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketing.Infrastructure.Data;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("refresh_tokens");
        entity.HasKey(rt => rt.Id);
        entity.Property(rt => rt.Id).HasColumnName("id");
        entity.Property(rt => rt.Token).HasColumnName("token").IsRequired();
        entity.Property(rt => rt.UserId).HasColumnName("user_id");
        entity.Property(rt => rt.ExpiresAt).HasColumnName("expires_at");
        entity.Property(rt => rt.CreatedAt).HasColumnName("created_at");
        entity.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");

        entity.HasIndex(rt => rt.Token).IsUnique();

        entity.Ignore(rt => rt.IsExpired);
        entity.Ignore(rt => rt.IsRevoked);
        entity.Ignore(rt => rt.IsActive);
    }
}
