using AcademyIO.Auth.API.Configuration;
using AcademyIO.Auth.API.Data;
using AcademyIO.WebAPI.Core.Configuration;
using AcademyIO.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogger(builder.Configuration);

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("live", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddDbContextCheck<ApplicationDbContext>(tags: new[] { "ready" });

builder.Services.AddSwaggerConfiguration();

builder.Services.AddMessageBusConfiguration(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwaggerSetup();

app.UseApiConfiguration(app.Environment);

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});

app.UseDbMigrationHelper();

app.Run();
