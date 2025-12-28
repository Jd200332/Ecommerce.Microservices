using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Product.Service.DTOs;
using Product.Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAll(int page = 1, int pagesize = 20)
        {
            var products = await productService.GetAllProductsAsync();
            return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResult(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(int id)
        {
            var product = await productService.GetProductByIdAsync(id);
            return Ok(ApiResponse<ProductResponse>.SuccessResult(product));
        }


        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetByCategory(int categoryId, int page = 1, int pagesize = 20)
        {
            var products = await productService.GetProductsByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResult(products));
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
