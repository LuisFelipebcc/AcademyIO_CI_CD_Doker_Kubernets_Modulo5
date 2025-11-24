using AcademyIO.Students.API.Configuration;
using AcademyIO.Students.API.Data;
using AcademyIO.WebAPI.Core.Configuration;
using AcademyIO.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddApiCoreConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddHealthChecks()
    .AddCheck("live", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddDbContextCheck<StudentsContext>(tags: new[] { "ready" });

builder.Services.AddSwaggerConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMessageBusConfiguration(builder.Configuration);
var app = builder.Build();

app.UseSwaggerSetup();
app.UseApiCoreConfiguration(app.Environment);

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
