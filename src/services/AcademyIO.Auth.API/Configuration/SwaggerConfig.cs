using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AcademyIO.Auth.API.Configuration
{
    /// <summary>
    /// Provides extension methods to register and configure Swagger/OpenAPI for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfig
    {
        /// <summary>
        /// Registers Swagger services and configures Swagger generation options, including XML comments
        /// and JWT bearer security definition.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the Swagger services to.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance to allow call chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(s =>
             {
                 var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 s.IncludeXmlComments(xmlPath);

                 s.SwaggerDoc("v1", new OpenApiInfo
                 {
                     Version = "v1",
                     Title = "Academy IO Auth",
                     Description = "AcademyIO Swagger",
                     Contact = new OpenApiContact { Name = "Academy IO Team", Email = "admin@AcademyIO.com" },
                     License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://github.com/ProfinProject/AcademyIO") }
                 });

                 s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                 {
                     Description = "Input the JWT like: Bearer {your token}",
                     Name = "Authorization",
                     Scheme = "Bearer",
                     BearerFormat = "JWT",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey
                 });

                 s.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                 });

             });

            return services;
        }

        /// <summary>
        /// Enables Swagger middleware and the Swagger UI in the application's request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure the middleware on.</param>
        /// <returns>The same <see cref="IApplicationBuilder"/> instance to allow call chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="app"/> is <c>null</c>.</exception>
        public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AcademyIO API v1");
            });

            return app;
        }
    }
}
