using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketing.Infrastructure.Data;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> entity)
    {
        entity.ToTable("profiles");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Id).HasColumnName("id");
        entity.Property(p => p.UserId).HasColumnName("user_id");
        entity.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
        entity.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
        entity.Property(p => p.CreatedAt).HasColumnName("created_at");
        entity.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        entity.HasIndex(p => p.UserId).IsUnique();

        // One-to-One: Profile -> User (configured in UserConfiguration)
        entity.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties for audit fields
        entity.Ignore(p => p.CreatedByUser);
        entity.Ignore(p => p.UpdatedByUser);
    }
}
