using Product.Service.Data;
using Microsoft.Extensions.Logging;
using Product.Service.DTOs;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using Product.Service.Models;


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

        public async Task<List<ProductResponse>> GetAllProductsAsync(int page = 1, int pagesize = 40,
            string? search = null,
            int? categoryId = null,
            decimal? minPrice = null,        // Min price filter
                decimal? maxPrice = null,        // Max price filter
                 bool? inStockOnly = null,        // Only show items with stock > 0
                 string? sortBy = null,           // "name", "price", "stock"
                bool sortDescending = false
            )


        {

            var query = context.Products.Where(p => p.IsActive);

            if(!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if(categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if(minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if(maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if(inStockOnly.HasValue && inStockOnly.Value)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

           
            var products = await (
                from p in context.Products
                join c in context.Categories on p.CategoryId equals c.Id
                select new ProductResponse
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CategoryName = c.Name,
                    CategoryDescription = c.Description

                })
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return products;

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
                   CategoryId = p.CategoryId

               })

               .ToListAsync();
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
           throw new NotImplementedException();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();    
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
