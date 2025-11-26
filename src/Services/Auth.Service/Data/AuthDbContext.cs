using Auth.Service.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>
                (entity =>
                {
                   entity.Property(e => e.FirstName).HasMaxLength(100);
                    entity.Property(e => e.LastName).HasMaxLength(100);
                    entity.Property(e => e.Address).HasMaxLength(200);
                    entity.Property(e => e.City).HasMaxLength(100);
                    entity.Property(e => e.State).HasMaxLength(100);
                    entity.Property(e => e.Country).HasMaxLength(100);
                    entity.Property(e => e.ZipCode).HasMaxLength(20);
                });
        }
    }
}