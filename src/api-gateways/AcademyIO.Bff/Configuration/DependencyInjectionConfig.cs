using AcademyIO.Bff.Extensions;
using AcademyIO.Bff.Services;
using AcademyIO.WebAPI.Core.Extensions;
using AcademyIO.WebAPI.Core.User;
using Polly;


namespace AcademyIO.Bff.Configuration
{
    /// <summary>
    /// Configures the dependency injection for the application.
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Registers services with the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAuthService, AuthService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IPaymentService, PaymentService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<IStudentService, StudentService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICourseService, CourseService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        }
    }
}
