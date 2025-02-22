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
        public virtual DbSet<VirtualCurrency> Currencies { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BetUser>()
                .HasMany(b => b.Currencies)
                .WithOne(c => c.BetUser)
                .HasForeignKey(c => c.BetUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
