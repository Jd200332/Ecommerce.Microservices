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

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync(int page = 1, int pagesize = 20)
        {

        }

        public async Task<ProductResponse> GetProductByIdAsync(int id)
        {
           
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pagesize = 20)
        {
            
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
