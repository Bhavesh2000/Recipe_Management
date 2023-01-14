using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Recipe_Management_System.Models;

namespace Recipe_Management_System.AppDbContext
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>()
            //    .Property(b => b.Type)
            //    .HasDefaultValue("User");

            modelBuilder.Entity<Recipe>()
                .Property(b => b.Status)
                .HasDefaultValue("Pending");
            modelBuilder.Entity<IdentityUser>()
                .Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<IdentityUser>()
                .Ignore(u => u.PhoneNumberConfirmed);

            modelBuilder.Ignore<IdentityRole>();
            modelBuilder.Ignore<IdentityUserToken<string>>();
            modelBuilder.Ignore<IdentityUserRole<string>>();
            modelBuilder.Ignore<IdentityUserLogin<string>>();
            modelBuilder.Ignore<IdentityUserClaim<string>>();
            modelBuilder.Ignore<IdentityRoleClaim<string>>();

            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; } 
    }
}
