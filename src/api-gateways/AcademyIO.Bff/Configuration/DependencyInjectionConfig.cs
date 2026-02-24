using AcademyIO.Bff.Extensions;
using AcademyIO.Bff.Services;
using AcademyIO.WebAPI.Core.Extensions;
using AcademyIO.WebAPI.Core.User;
using Polly;
using Polly.Extensions.Http;


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
        /// <param name="configuration">The IConfiguration to read settings from (optional).</param>
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            var circuitBreakerTimeout = configuration?.GetValue<int>("Polly:CircuitBreakerSeconds") ?? 30;
            var retryCount = configuration?.GetValue<int>("Polly:RetryCount") ?? 3;

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient<IAuthService, AuthService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(retryPolicy)
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(circuitBreakerTimeout)));

            services.AddHttpClient<IPaymentService, PaymentService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(retryPolicy)
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(circuitBreakerTimeout)));

            services.AddHttpClient<IStudentService, StudentService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(retryPolicy)
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(circuitBreakerTimeout)));

            services.AddHttpClient<ICourseService, CourseService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(retryPolicy)
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(circuitBreakerTimeout)));
        }
    }
}
