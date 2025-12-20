using Microsoft.EntityFrameworkCore;
using Product.Service.Models;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Product.Service.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options)
        {
        }


        public DbSet<Product.Service.Models.Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product.Service.Models.Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
            });


            modelBuilder.Entity<Category>().HasData
                (
                new Category { Id = 1, Name = "Electronics", Description = "Electronic items" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion" },
                new Category { Id = 3, Name = "Books", Description = "Books and magazines" }
                );
        }
    }
}