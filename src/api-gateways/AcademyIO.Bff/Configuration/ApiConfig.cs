using AcademyIO.Bff.Extensions;

namespace AcademyIO.Bff.Configuration
{
    /// <summary>
    /// Configures the API services and pipeline.
    /// </summary>
    public static class ApiConfig
    {
        /// <summary>
        /// Adds API services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.Configure<AppServicesSettings>(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            //TO DO ???
            //services.AddDefaultHealthCheck(configuration)
            //    .AddUrlGroup(new Uri($"{configuration["CourseUrl"]}/healthz-infra"), "Shopping Cart", tags: new[] { "infra" }, configureHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler())
            //    .AddUrlGroup(new Uri($"{configuration["StudentUrl"]}/healthz-infra"), "Catalog API", tags: new[] { "infra" }, configureHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler())
            //    .AddUrlGroup(new Uri($"{configuration["PaymentUrl"]}/healthz-infra"), "Billing API", tags: new[] { "infra" }, configureHttpMessageHandler: _ => HttpExtensions.ConfigureClientHandler());
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The WebApplication to configure.</param>
        /// <param name="env">The web hosting environment.</param>
        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Under certain scenarios, e.g minikube / linux environment / behind load balancer
            // https redirection could lead dev's to over complicated configuration for testing purpouses
            // In production is a good practice to keep it true
            if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Total");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //app.UseDefaultHealthcheck();
        }
    }
}