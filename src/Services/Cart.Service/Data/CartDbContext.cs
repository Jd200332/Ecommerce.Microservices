using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cart.Service.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cart.Service.Models.Cart> Cart { get; set; }

        public DbSet<Cart.Service.Models.CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart.Service.Models.Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            modelBuilder.Entity<Cart.Service.Models.CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Cart)
                       .WithMany(c => c.CartItems)
                       .HasForeignKey(e => e.CartId);
            });
        }
    }
}