using Product.Service.Data;
using Microsoft.Extensions.Logging;
using Product.Service.DTOs;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using Product.Service.Models;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography.Xml;
using System.Data;
using System.Runtime.InteropServices;


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

        public async Task<List<ProductResponse>> GetAllProductsAsync(int page = 1, int pagesize = 10,
            string? search = null,
            int? categoryId = null,
            decimal? minPrice = null,        // Min price filter
                decimal? maxPrice = null,        // Max price filter
                 bool? inStockOnly = null,        // Only show items with stock > 0
                 string? sortBy = null,           // "name", "price", "stock"
                bool sortDescending = false
            )


        {
            var query = (
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

                });

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

            query = sortBy?.ToLower() switch
            {
                "price" => sortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "stock" => sortDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Id)
            };

            return await query
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

        }

        public async Task<ProductResponse> GetProductByIdAsync(int id, int page = 1, int pagesize = 10)
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
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .AsNoTracking()
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
            .FirstOrDefaultAsync();
            

            return result;
        }

        public async Task<List<ProductResponse>> GetProductsByCategoryAsync(int categoryId,  int page = 1, int pagesize = 10)
        {
            if (categoryId <= 0)
            {
                throw new ArgumentException("Invalid category", nameof(categoryId));
            }

            

            var query = context.Products.Where(x => x.CategoryId == categoryId && x.IsActive);
            var totalcounts = await query.CountAsync();


            
            var pbyc = await (
                from p in context.Products
                join c in context.Categories
                on p.CategoryId equals c.Id
                where p.CategoryId == categoryId && p.IsActive
                orderby p.Id ascending

                select new ProductResponse
                
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = c.Name

                })
                .AsNoTracking()
                .Skip((page - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            return pbyc;
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            var category = await context.Categories.FindAsync(request.CategoryId);

            if (category == null)
                throw new Exception("Invalid Categoryid");

            if (request.Name == null)
            {
                throw new InvalidOperationException("Name cannot be null");
            }


            if(request.Price <= 0 )
            {
                throw new InvalidOperationException("Price cannot be less than 0");
            }

            if(request.StockQuantity <= 0)
            {
                throw new ArgumentException("Stock quantity cannot be 0");
            }


            var product = new Product.Service.Models.Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    ImageUrl = request.ImageUrl

                };

            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();

            }

            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error storing data {Message}", ex.Message);
                throw new Exception("An error occurred");
            }


            return new ProductResponse
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CategoryName = category.Name,
                CategoryDescription = category.Description
            };
        }


        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken ct = default)
        { 
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existingproduct = await context.Products.FindAsync(id);

            if (existingproduct == null)
                throw new KeyNotFoundException($"Product {id} not found");
                
            existingproduct.Name = request.Name;
            existingproduct.Description = request.Description;
            existingproduct.Price = request.Price ?? existingproduct.Price;
            existingproduct.StockQuantity = request.StockQuantity ?? existingproduct.StockQuantity;
            existingproduct.CategoryId = request.CategoryId ?? existingproduct.CategoryId;
            existingproduct.ImageUrl = request.ImageUrl;

            try 
            {
                await context.SaveChangesAsync(ct);
            }

            catch (DbUpdateConcurrencyException)
            {
                throw new DBConcurrencyException($"Product {id} was modified by another user");
            }

            catch(DbUpdateException ex)
            {
                logger.LogError(ex, "Failed to update product {ProductId}", id);
                throw;
            }
            
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken ct = default)
        {
            var product = await context.Products.FindAsync(id);

            if (product == null)
                throw new ArgumentNullException("product cannot be null");

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return false;

        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
