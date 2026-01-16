using System.Text;
using DotNetEnv;
using EventTicketing.Application.Services;
using EventTicketing.Hub;
using EventTicketing.Infrastructure.Configuration;
using EventTicketing.Infrastructure.Data;
using EventTicketing.API.Extensions;
using EventTicketing.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// Configure Services
// ===========================================

// JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

// Controllers with filters
builder.Services.AddApiControllers(options =>
{
    options.Filters.Add<ApiResponseAuthorizationFilter>();
});

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// Health Checks
builder.Services.AddHealthChecking();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Success = false,
                Message = "Unauthorized access",
                Status = "Unauthorized",
                Errors = Array.Empty<object>(),
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path.ToString()
            });
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                Success = false,
                Message = "Access forbidden",
                Status = "Forbidden",
                Errors = Array.Empty<object>(),
                Timestamp = DateTime.UtcNow,
                Path = context.Request.Path.ToString()
            });
        }
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<EventConnectionTracker>();

// Database
builder.Services.AddDatabase(builder.Configuration);

// Application Services
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<DataSeeder>();

// ===========================================
// Build Application
// ===========================================

var app = builder.Build();

// ===========================================
// Configure Middleware Pipeline
// ===========================================

// Development only
if (app.Environment.IsDevelopment())
{
    // Seed database
    // using var scope = app.Services.CreateScope();
    // var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    // await seeder.SeedAsync();

    // OpenAPI/ReDoc
    app.MapOpenApi();
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
        options.RoutePrefix = "api-docs";
    });
    
    // Redirect root to API docs (excluded from OpenAPI)
    app.MapGet("/", () => Results.Redirect("/api-docs")).ExcludeFromDescription();
}

// Static files (UI)
app.UseDefaultFiles();
app.UseStaticFiles();

// Security
app.UseHttpsRedirection();

// Error handling
app.UseGlobalExceptionHandling();

// CORS
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapApiControllers();
app.MapHub<EventHub>("/eventhub");

// ===========================================
// Run Application
// ===========================================

app.Run();
