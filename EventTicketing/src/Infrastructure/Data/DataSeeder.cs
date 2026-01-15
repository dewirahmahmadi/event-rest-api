using EventTicketing.Domain.Entities;
using EventTicketing.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventTicketing.Infrastructure.Data;

public class DataSeeder
{
    private readonly DataDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(DataDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();

            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already seeded. Skipping...");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            var adminUsers = await SeedAdminUsersAsync();
            var guestUsers = await SeedGuestUsersAsync();
            var allUsers = adminUsers.Concat(guestUsers).ToList();

            await SeedProfilesAsync(allUsers);
            var events = await SeedEventsAsync(adminUsers.First());
            await SeedEventRegistrationsAsync(guestUsers, events);

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task<List<User>> SeedAdminUsersAsync()
    {
        var adminUsers = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@eventticketing.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "superadmin@eventticketing.com",
                Password = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(adminUsers);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} admin users.", adminUsers.Count);
        return adminUsers;
    }

    private async Task<List<User>> SeedGuestUsersAsync()
    {
        var guestUsers = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "john.doe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                Role = UserRole.Guest,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "jane.smith@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                Role = UserRole.Guest,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "bob.wilson@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                Role = UserRole.Guest,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "alice.johnson@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                Role = UserRole.Guest,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "charlie.brown@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Guest@123"),
                Role = UserRole.Guest,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(guestUsers);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} guest users.", guestUsers.Count);
        return guestUsers;
    }

    private async Task SeedProfilesAsync(List<User> users)
    {
        var profileData = new Dictionary<string, (string FirstName, string LastName)>
        {
            { "admin@eventticketing.com", ("System", "Administrator") },
            { "superadmin@eventticketing.com", ("Super", "Admin") },
            { "john.doe@example.com", ("John", "Doe") },
            { "jane.smith@example.com", ("Jane", "Smith") },
            { "bob.wilson@example.com", ("Bob", "Wilson") },
            { "alice.johnson@example.com", ("Alice", "Johnson") },
            { "charlie.brown@example.com", ("Charlie", "Brown") }
        };

        var profiles = users.Select(user =>
        {
            var (firstName, lastName) = profileData.GetValueOrDefault(
                user.Email,
                ("Unknown", "User")
            );

            return new Profile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }).ToList();

        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} profiles.", profiles.Count);
    }

    private async Task<List<Event>> SeedEventsAsync(User adminUser)
    {
        var now = DateTime.UtcNow;

        var events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Tech Conference 2025",
                Description = "Annual technology conference featuring the latest innovations in AI, cloud computing, and software development. Join industry experts for insightful talks and networking opportunities.",
                Location = "Convention Center, Hall A",
                StartDate = now.AddDays(30),
                EndDate = now.AddDays(32),
                MaxAttendees = 500,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Startup Pitch Night",
                Description = "Watch promising startups pitch their ideas to a panel of investors. Network with entrepreneurs and discover the next big thing in tech.",
                Location = "Innovation Hub, Main Stage",
                StartDate = now.AddDays(14),
                EndDate = now.AddDays(14).AddHours(4),
                MaxAttendees = 150,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Web Development Workshop",
                Description = "Hands-on workshop covering modern web development practices including React, Node.js, and cloud deployment strategies.",
                Location = "Tech Campus, Room 201",
                StartDate = now.AddDays(7),
                EndDate = now.AddDays(7).AddHours(6),
                MaxAttendees = 50,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Cybersecurity Summit",
                Description = "Learn about the latest cybersecurity threats and defense strategies from industry professionals. Essential for IT professionals and security enthusiasts.",
                Location = "Security Center, Auditorium",
                StartDate = now.AddDays(45),
                EndDate = now.AddDays(46),
                MaxAttendees = 300,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Data Science Meetup",
                Description = "Monthly meetup for data scientists and analysts. Share insights, discuss trends, and collaborate on exciting projects.",
                Location = "Data Hub, Conference Room B",
                StartDate = now.AddDays(10),
                EndDate = now.AddDays(10).AddHours(3),
                MaxAttendees = 75,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Mobile App Development Bootcamp",
                Description = "Intensive bootcamp covering iOS and Android development. Build your first mobile app from scratch with expert guidance.",
                Location = "Developer Academy, Lab 3",
                StartDate = now.AddDays(21),
                EndDate = now.AddDays(23),
                MaxAttendees = 30,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Cloud Architecture Workshop",
                Description = "Deep dive into cloud architecture patterns using AWS, Azure, and GCP. Learn best practices for scalable and resilient systems.",
                Location = "Cloud Center, Training Room 1",
                StartDate = now.AddDays(60),
                EndDate = now.AddDays(61),
                MaxAttendees = 40,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "AI & Machine Learning Expo",
                Description = "Explore the cutting edge of artificial intelligence and machine learning. Demos, talks, and hands-on experiences with the latest AI tools.",
                Location = "AI Innovation Center",
                StartDate = now.AddDays(90),
                EndDate = now.AddDays(92),
                MaxAttendees = 1000,
                CurrentAttendeeCount = 0,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await _context.Events.AddRangeAsync(events);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} events.", events.Count);
        return events;
    }

    private async Task SeedEventRegistrationsAsync(List<User> guestUsers, List<Event> events)
    {
        var registrations = new List<EventRegistration>();
        var random = new Random(42); // Fixed seed for reproducibility

        // Track unique user-event combinations
        var registeredPairs = new HashSet<(Guid UserId, Guid EventId)>();

        foreach (var guest in guestUsers)
        {
            // Each guest registers for 2-4 random events
            var numberOfRegistrations = random.Next(2, 5);

            // Shuffle events and select unique ones for this user
            var availableEvents = events.ToList();
            var registeredCount = 0;

            while (registeredCount < numberOfRegistrations && availableEvents.Count > 0)
            {
                var randomIndex = random.Next(availableEvents.Count);
                var evt = availableEvents[randomIndex];

                // Check if this user-event pair already exists
                var pair = (guest.Id, evt.Id);
                if (!registeredPairs.Contains(pair))
                {
                    registeredPairs.Add(pair);

                    var registration = new EventRegistration
                    {
                        Id = Guid.NewGuid(),
                        UserId = guest.Id,
                        EventId = evt.Id,
                        RegisteredAt = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                        IsAttending = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    registrations.Add(registration);
                    evt.CurrentAttendeeCount++;
                    registeredCount++;
                }

                // Remove from available events to avoid checking again
                availableEvents.RemoveAt(randomIndex);
            }
        }

        await _context.EventRegistrations.AddRangeAsync(registrations);
        _context.Events.UpdateRange(events);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} unique event registrations.", registrations.Count);
    }
}
