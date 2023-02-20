using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectCMS.Models;

namespace ProjectCMS.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        DbSet<Category> _categories { get; set; }
        DbSet<Comment> _comments { get; set; }
        DbSet<Department> _departments { get; set; }
        DbSet<Event> _events { get; set; }
        DbSet<Idea> _idea { get; set; }
        DbSet<Interactions> _interactions { get; set; }
        DbSet<User> _users { get; set; }
    }
}