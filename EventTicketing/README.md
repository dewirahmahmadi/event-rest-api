# Event Ticketing API

A modern ASP.NET Core 9.0 Web API for event ticket management with authentication, registration, and real-time notifications.

## Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL (via Supabase)
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: ReDoc / OpenAPI
- **Real-time**: SignalR
- **Password Hashing**: BCrypt.Net

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL Database (or Supabase account)
- Docker (optional, for containerized deployment)

## Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd EventTicketing/EventTicketing
```

### 2. Configure Database Connection

Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-host;Port=5432;Database=your-database;Username=your-username;Password=your-password;SSL Mode=Require;"
  }
}
```

Or create `appsettings.Development.json` for local development:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=eventticketing;Username=postgres;Password=yourpassword"
  }
}
```

### 4. Run Database Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update
```

### 5. Restore Dependencies

```bash
dotnet restore
```

## Running the Project

### Development Mode

```bash
# Run the application
dotnet run

# Run with HTTPS profile
dotnet run --launch-profile https

# Run with HTTP profile
dotnet run --launch-profile http
```

The API will be available at:
- HTTP: `http://localhost:5228`
- HTTPS: `https://localhost:7025`
- API Documentation: `http://localhost:5228/api-docs` or `https://localhost:7025/api-docs`

### Docker Deployment

```bash
# Build Docker image
docker build -t eventticketing .

# Run container
docker run -p 8080:8080 -p 8081:8081 eventticketing
```

Or use Docker Compose:

```bash
docker-compose up --build
```

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/signup` | Register new user | No |
| POST | `/login` | Login with email/password | No |
| POST | `/refresh` | Refresh access token | No |
| POST | `/logout` | Logout (revoke token) | Yes |
| GET | `/me` | Get current user info | Yes |

### Events (`/api/event`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get all events (paginated) | No |
| GET | `/upcoming` | Get upcoming events (paginated) | No |
| GET | `/{id}` | Get event by ID | No |
| POST | `/` | Create new event | Admin |
| PUT | `/{id}` | Update existing event | Admin |
| DELETE | `/{id}` | Delete event | Admin |

### Registrations (`/api/registration`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/` | Register for event | Guest |
| GET | `/` | Get all registrations (paginated) | Admin |
| GET | `/{id}` | Get registration by ID | Admin |
| GET | `/event/{eventId}` | Get registrations by event | Admin |
| PUT | `/{id}` | Update registration | Admin |
| DELETE | `/{id}` | Delete registration | Admin |
| PATCH | `/{id}/checkin` | Check-in registration | Admin |
| PATCH | `/{id}/checkout` | Check-out registration | Admin |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | System health status |

## Development

### Building the Project

```bash
dotnet build
```

## License

This project is proprietary software.

## Support

For issues and questions, please contact the development team.