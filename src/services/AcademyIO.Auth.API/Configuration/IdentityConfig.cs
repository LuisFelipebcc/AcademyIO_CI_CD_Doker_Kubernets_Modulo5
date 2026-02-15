using AcademyIO.Auth.API.Data;
using AcademyIO.WebAPI.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;

using static AcademyIO.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace AcademyIO.Auth.API.Configuration
{
    /// <summary>
    /// Provides extension methods to configure ASP.NET Core Identity for the application.
    /// </summary>
    public static class IdentityConfig
    {
        /// <summary>
        /// Adds and configures Identity services, related managers and providers.
        /// Registers the EF Core provider for <see cref="ApplicationDbContext"/>, memory cache,
        /// data protection and default Identity with role support.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The application <see cref="IConfiguration"/> used to detect the database provider.</param>
        /// <returns>The original <see cref="IServiceCollection"/> with Identity services added.</returns>
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureProviderForContext<ApplicationDbContext>(DetectDatabase(configuration), "AcademyIO.Auth.API");

            services.AddMemoryCache()
                .AddDataProtection();

            services.AddDefaultIdentity<IdentityUser<Guid>>()
             .AddRoles<IdentityRole<Guid>>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddSignInManager()
             .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
             .AddDefaultTokenProviders();

            return services;
        }
    }
}
