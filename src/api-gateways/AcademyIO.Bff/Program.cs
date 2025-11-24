using AcademyIO.Bff.Configuration;
using AcademyIO.Bff.Extensions;
using AcademyIO.WebAPI.Core.Identity;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

#region Configure Services


builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.AddHealthChecks()
    .AddCheck("auth-api-check", () =>
    {
        // Implement a custom health check or use a simple ping
        return HealthCheckResult.Healthy("Auth API is healthy (placeholder)");
    })
    .AddCheck("courses-api-check", () =>
    {
        return HealthCheckResult.Healthy("Courses API is healthy (placeholder)");
    })
    .AddCheck("students-api-check", () =>
    {
        return HealthCheckResult.Healthy("Students API is healthy (placeholder)");
    })
    .AddCheck("payments-api-check", () =>
    {
        return HealthCheckResult.Healthy("Payments API is healthy (placeholder)");
    });

builder.Services.RegisterServices();

builder.Services.Configure<AppServicesSettings>(builder.Configuration.GetSection("AppServicesSettings"));
//builder.Services.AddMessageBusConfiguration(builder.Configuration);

//builder.Services.ConfigureGrpcServices(builder.Configuration);


var app = builder.Build();
#endregion

#region Configure Pipeline


app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

#endregion