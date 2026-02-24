namespace AcademyIO.WebAPI.Core.Configuration
{
    public static class OpenTelemetryConfig
    {
        public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration, string serviceName)
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .AddResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(configuration["OpenTelemetry:ExporterUrl"] ?? "http://otel-collector:4317");
                        });
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(configuration["OpenTelemetry:ExporterUrl"] ?? "http://otel-collector:4317");
                        });
                });

            return services;
        }
    }
}