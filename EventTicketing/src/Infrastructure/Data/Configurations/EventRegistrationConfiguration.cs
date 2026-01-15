using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketing.Infrastructure.Data;

public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> entity)
    {
        entity.ToTable("event_registrations");
        entity.HasKey(er => er.Id);
        entity.Property(er => er.Id).HasColumnName("id");
        entity.Property(er => er.UserId).HasColumnName("user_id");
        entity.Property(er => er.EventId).HasColumnName("event_id");
        entity.Property(er => er.RegisteredAt).HasColumnName("registered_at");
        entity.Property(er => er.IsAttending).HasColumnName("is_attending");
        entity.Property(er => er.CheckedInAt).HasColumnName("checked_in_at");
        entity.Property(er => er.CheckedOutAt).HasColumnName("checked_out_at");
        entity.Property(er => er.CreatedAt).HasColumnName("created_at");
        entity.Property(er => er.UpdatedAt).HasColumnName("updated_at");

        entity.HasIndex(er => new { er.EventId, er.UserId }).IsUnique();

        // Many-to-One: EventRegistration -> Event
        entity.HasOne(er => er.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(er => er.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-One: EventRegistration -> User
        entity.HasOne(er => er.User)
            .WithMany(u => u.EventRegistrations)
            .HasForeignKey(er => er.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties for audit fields
        entity.Ignore(er => er.CreatedByUser);
        entity.Ignore(er => er.UpdatedByUser);
    }
}
