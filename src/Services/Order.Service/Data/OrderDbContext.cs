using Microsoft.EntityFrameworkCore;
using Order.Service.Models;
using Microsoft.EntityFrameworkCore.Design;


namespace Order.Service.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }


        public DbSet<Order.Service.Models.Order> Orders { get; set; }
        public DbSet<Order.Service.Models.OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order.Service.Models.Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Order.Service.Models.OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Order)
                       .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId);
            });
        }

    }
}
