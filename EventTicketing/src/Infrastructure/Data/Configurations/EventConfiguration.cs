using EventTicketing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventTicketing.Infrastructure.Data;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.ToTable("events");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Title).HasColumnName("title").IsRequired();
        entity.Property(e => e.Description).HasColumnName("description");
        entity.Property(e => e.StartDate).HasColumnName("start_date");
        entity.Property(e => e.EndDate).HasColumnName("end_date");
        entity.Property(e => e.Location).HasColumnName("location");
        entity.Property(e => e.MaxAttendees).HasColumnName("max_attendees");
        entity.Property(e => e.CurrentAttendeeCount).HasColumnName("current_attendee_count");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

        // One-to-Many: Event -> EventRegistrations
        entity.HasMany(e => e.Registrations)
            .WithOne(er => er.Event)
            .HasForeignKey(er => er.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties for audit fields
        entity.Ignore(e => e.CreatedByUser);
        entity.Ignore(e => e.UpdatedByUser);
    }
}
