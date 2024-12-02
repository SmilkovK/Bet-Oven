using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SportDomain.models;
using System.Security.Cryptography.X509Certificates;

namespace Bet_Oven.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {

        public virtual DbSet<Currency> Currencies { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
