using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product.Service.DTOs;
using Product.Service.Services;
using System.Threading.Tasks;

namespace Product.Service.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetAllProductsAsync(int page = 1, int pagesize = 20)
        {
            var products =   productService.GetAllProductsAsync(page, pagesize);
            var result = await products.ToListAsync();
            return Ok(ApiResponse<IQueryable<ProductResponse>>.SuccessResult(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductByIdAsync(int id)
        {
            var result = await productService.GetProductByIdAsync(id);

            if(result == null)
            {
                return NotFound(ApiResponse<ProductResponse>.ErrorResult($"Product {id} not found"));
            }
            return Ok(ApiResponse<ProductResponse>.SuccessResult(result));
        }


        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetProdutsByCategoryAsync(int categoryId, int page = 1, int pagesize = 20)
        {
            var products = await productService.GetProductsByCategoryAsync(categoryId, page, pagesize); 
             
            if(products == null || products.Count == 0)
            {
                return Ok(ApiResponse<List<ProductResponse>>.ErrorResult
                    ($"No products found for category {categoryId}"));
            }
            return Ok(ApiResponse<List<ProductResponse>>.SuccessResult(products));
        }

        [HttpPost]

        public async Task<ActionResult<ApiResponse<ProductResponse>>> Create(CreateProductRequest request)
        {
            var product = await productService.CreateProductAsync(request);
            return Ok(ApiResponse<ProductResponse>.SuccessResult(product, "Product created"));
        }

        [HttpPut("{id}")]
        
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateProductRequest request)
        {
            var result = await productService.UpdateProductAsync(id, request);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Product updated"));
        }

        [HttpDelete("{id}")]
        
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await productService.DeleteProductAsync(id);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Product deleted"));
        }

        [HttpPatch("{id}/stock")]
        
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStock(int id, [FromBody] int quantity)
        {
            var result = await productService.UpdateStockAsync(id, quantity);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Stock updated"));
        }


    }
}
