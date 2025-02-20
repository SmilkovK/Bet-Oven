using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SportDomain.Identity;
using SportDomain.models;
using System.Security.Cryptography.X509Certificates;

namespace SportRepository
{

    public class ApplicationDbContext : IdentityDbContext<BetUser>
    {
        public virtual DbSet<Currency> Currencies { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
