using EventTicketing.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventTicketing.Infrastructure.Services;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly DataDbContext _dbContext;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(DataDbContext dbContext, ILogger<DatabaseHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to connect to the database
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }

            var healthData = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["status"] = "connected",
                ["provider"] = "PostgreSQL"
            };

            return HealthCheckResult.Healthy("Database is healthy", healthData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}