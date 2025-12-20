using Product.Service.Data;
using Microsoft.Extensions.Logging;
using Product.Service.DTOs;
using System;
using Microsoft.EntityFrameworkCore;

namespace Product.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext context;
        private readonly ILogger<ProductService> logger;    

        public ProductService(ProductDbContext context,
            ILogger<ProductService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            return await context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,  
                    CategoryName = p.Category.Name, 
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
            var product = await context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId)
        {
            return await context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Models.Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl,
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id);
        }


        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await context.Products.FindAsync(id);
            if(product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.Name = request.Name ?? product.Name;
            product.Description = request.Description ?? product.Description;   
            product.Price = request.Price ?? product.Price;
            product.StockQuantity = request.StockQuantity ?? product.StockQuantity;
            product.CategoryId = request.CategoryId ?? product.CategoryId;
            product.ImageUrl = request.ImageUrl ?? product.ImageUrl;
            product.IsActive = request.IsActive ?? product.IsActive;
            product.UpdateAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.IsActive = false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            var product = await context.Products.FindAsync(productId);
            if(product == null)
                throw new KeyNotFoundException("Product not found");

            product.StockQuantity += quantity;
            await context.SaveChangesAsync();
            return true;

        }
    }
}
