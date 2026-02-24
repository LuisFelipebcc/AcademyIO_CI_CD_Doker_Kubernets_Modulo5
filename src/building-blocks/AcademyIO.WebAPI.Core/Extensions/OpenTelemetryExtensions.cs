using Microsoft.Extensions.DependencyInjection;

namespace AcademyIO.WebAPI.Core.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services)
        {
            return services;
        }
    }
}