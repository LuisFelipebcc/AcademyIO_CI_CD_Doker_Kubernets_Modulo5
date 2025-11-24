using AcademyIO.WebAPI.Core.Configuration;
using AcademyIO.WebAPI.Core.User;

namespace AcademyIO.Auth.API.Configuration
{
    /// <summary>
    /// Provides extension methods to configure API services and the application pipeline.
    /// </summary>
    public static class ApiConfig
    {
        /// <summary>
        /// Registers controllers, user services, health checks and CORS policies required by the API.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddDefaultHealthCheck(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            return services;
        }

        /// <summary>
        /// Configures the application's HTTP request pipeline with routing, CORS, authorization and health checks.
        /// Also enables Swagger in development and optionally redirects HTTP to HTTPS based on configuration.
        /// </summary>
        /// <param name="app">The <see cref="WebApplication"/> instance to configure.</param>
        /// <param name="env">The hosting environment information.</param>
        /// <returns>The configured <see cref="IApplicationBuilder"/> for further chaining.</returns>
        public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Under certain scenarios, e.g. minikube / linux environment / behind load balancer
            // https redirection could lead dev's to overcomplicated configuration for testing purposes
            // In production is a good practice to keep it true
            if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Total");

            //TODO ???
            //app.UseAuthConfiguration();
            app.UseAuthorization();
            app.MapControllers();


            app.UseDefaultHealthcheck();

            return app;
        }
    }
}