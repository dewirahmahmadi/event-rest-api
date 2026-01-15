using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketing.Infrastructure.Data;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Id).HasColumnName("id");
        entity.Property(u => u.Email).HasColumnName("email").IsRequired();
        entity.Property(u => u.Password).HasColumnName("password").IsRequired();
        entity.Property(u => u.Role)
            .HasColumnName("role")
            .HasConversion<string>();
        entity.Property(u => u.IsActive).HasColumnName("is_active");
        entity.Property(u => u.CreatedAt).HasColumnName("created_at");
        entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");

        entity.HasIndex(u => u.Email).IsUnique();

        // One-to-One: User -> Profile
        entity.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: User -> RefreshTokens
        entity.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: User -> EventRegistrations (Many-to-Many with Event via EventRegistration)
        entity.HasMany(u => u.EventRegistrations)
            .WithOne(er => er.User)
            .HasForeignKey(er => er.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties for audit fields to avoid circular references
        entity.Ignore(u => u.CreatedByUser);
        entity.Ignore(u => u.UpdatedByUser);
    }
}
