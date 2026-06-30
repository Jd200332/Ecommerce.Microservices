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

        public DbSet<Order.Service.Models.Outboxmessage> Outboxmessages { get; set; }

        public DbSet<Order.Service.Models.Orderpayment> Orderpayments { get; set; }

        public DbSet<Order.Service.Models.Orderstatushistory> Orderstatushistory {  get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Order.Service.Models.Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasMany(o => o.OrderItems)
                      .WithOne(i => i.Order)
                      .HasForeignKey(i => i.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(o => o.OrderPayments)           // ← NEW
                      .WithOne(p => p.Order)
                      .HasForeignKey(p => p.OrderId);
                entity.HasMany(o => o.StatusHistories)         // ← NEW
                      .WithOne(h => h.Order)
                      .HasForeignKey(h => h.OrderId);
            });

            
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            });

            
            modelBuilder.Entity<Outboxmessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Status, e.CreatedAt });
                entity.Property(e => e.EventPayload).HasColumnType("nvarchar(max)");
            });

            //  payment 
            modelBuilder.Entity<Orderpayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PaymentIntentId).IsUnique();
            });

            // STATUS history 
            modelBuilder.Entity<Orderstatushistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.OrderId, e.ChangedAt });
            });
        }
    }
}
