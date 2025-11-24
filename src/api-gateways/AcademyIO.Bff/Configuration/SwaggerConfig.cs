using Microsoft.OpenApi.Models;

namespace AcademyIO.Bff.Configuration
{
    /// <summary>
    /// Configures Swagger/OpenAPI for the application.
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Adds Swagger generation services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "AcademyIO BFF - API Gateway",
                    Description = "This API is part of online course ASP.NET Core Enterprise Applications.",
                    Contact = new OpenApiContact() { Name = "AcademyIO", Email = "contato@academy.io" },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/Licenses/MIT") }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Add you JWT token: Bearer {token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });


                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        }

        /// <summary>
        /// Configures the HTTP request pipeline to use Swagger and Swagger UI.
        /// </summary>
        /// <param name="app">The IApplicationBuilder to configure.</param>
        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
    }
}
