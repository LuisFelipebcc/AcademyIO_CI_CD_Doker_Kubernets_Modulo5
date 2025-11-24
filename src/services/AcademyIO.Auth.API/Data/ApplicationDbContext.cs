using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcademyIO.Auth.API.Data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the authentication API.
    /// Uses ASP.NET Core Identity types with <see cref="Guid"/> primary keys.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="IdentityDbContext{TUser,TRole,TKey}"/> to provide Identity stores.
    /// Add DbSet properties for application-specific entities here as needed.
    /// </remarks>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the database context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
