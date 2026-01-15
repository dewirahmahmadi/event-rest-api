using System.Reflection;
using EventTicketing.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.Infrastructure.Configuration;

public static class ControllerExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services, Action<MvcOptions>? configure = null)
    {
        // Add controllers and API explorer for OpenAPI
        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
            configure?.Invoke(options);
        });
        
        // Add API explorer for OpenAPI/Swagger
        services.AddEndpointsApiExplorer();
        
        return services;
    }
    
    public static IServiceCollection AddHealthChecking(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck("self", () =>
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

        return services;
    }
    
    public static WebApplication MapApiControllers(this WebApplication app)
    {
        // Map all controllers automatically
        app.MapControllers();
        
        // Map health checks
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        exception = x.Value.Exception?.Message,
                        duration = x.Value.Duration,
                        data = x.Value.Data
                    }),
                    duration = report.TotalDuration
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }));
            }
        });
        
        return app;
    }
}