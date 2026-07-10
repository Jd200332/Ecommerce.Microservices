using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Cart.Service.Models;

namespace Cart.Service.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {
        }

        public DbSet<CartData> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartData>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Fix: Not unique—one user can have multiple carts (Active, CheckedOut, etc.)
                entity.HasIndex(e => e.UserId)
                      .IsUnique(false);  // Or remove IsUnique() entirely

                // Indexes for common queries
                entity.Property(e => e.SessionId)
                      .HasMaxLength(450);

                entity.HasIndex(e => e.SessionId); // For session-based lookups 

                entity.HasIndex(e => e.Status); // For cleanup jobs

                // Decimal precision
                entity.Property(e => e.SubTotal)
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total)
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount)
                      .HasColumnType("decimal(18,2)");

                // RowVersion as concurrency token
                
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Fix: Use correct property name
                entity.Property(e => e.LockedPrice)
                      .HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.CartId);
                entity.HasIndex(e => e.ProductId);  



                // Cascade delete
                entity.HasOne<CartData>()
                      .WithMany()
                      .HasForeignKey(e => e.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
