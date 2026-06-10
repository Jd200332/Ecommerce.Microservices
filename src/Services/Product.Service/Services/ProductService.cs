using Product.Service.Data;
using Microsoft.Extensions.Logging;
using Product.Service.DTOs;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;

namespace Product.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext context;
        private readonly ILogger<ProductService> logger;


        public ProductService(ProductDbContext context,
            ILogger<ProductService> logger
            
            )
        {
            this.context = context;
            this.logger = logger;

        }

        public IQueryable<ProductResponse> GetAllProductsAsync(int page = 1, int pagesize = 20)
        {

            if (page <= 0)
            {
                page = 1;
            }

            if (pagesize <= 0)
            {
                pagesize = 20;
            }

            else if (pagesize > 100)
            {
                pagesize = 100;
            }

            return context.Products
                .Where(p => p.IsActive)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,    
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                   
                });
        }

        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
            var result = await context.Products
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl
                })

            .FirstOrDefaultAsync();
            return result;

        }

        public async Task<List<ProductResponse>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pagesize = 20)
        {
            return await context.Products
               .Where(p => p.CategoryId == categoryId && p.IsActive)
               .Skip((page - 1) * pagesize)
               .Take(pagesize)
               .Select
               (p => new ProductResponse
               {
                   Id = p.Id,
                   Name = p.Name,
                   Description = p.Description,
                   Price = p.Price,
                   StockQuantity = p.StockQuantity,
                   

               })

               .ToListAsync();
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            
        }


        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
           
        }

        public async Task<bool> DeleteProductAsync(int id)
        {

        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {

        }
    }
}
