using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product.Service.DTOs;
using Product.Service.Services;
using System.Threading.Tasks;
using Product.Service.Models;
using Product.Service.Data;
using Product.Service.Controllers;



//Jd200332 password dockerusername and dockerpassword
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
        public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetAllProductsAsync(int page = 1, int pagesize = 40)
        {
            // quick diagnostic log
            var logger = HttpContext.RequestServices.GetService(typeof(Microsoft.Extensions.Logging.ILogger<ProductController>))
                         as Microsoft.Extensions.Logging.ILogger;
            logger?.LogInformation("GetAllProducts called with page={Page} pagesize={PageSize}", page, pagesize);

            var products = await productService.GetAllProductsAsync(page, pagesize);
            return Ok(ApiResponse<List<ProductResponse>>.SuccessResult(products));
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
            var pbyc = await productService.GetProductsByCategoryAsync(categoryId, page, pagesize); 
             
            if(pbyc == null || pbyc.Count == 0)
            {
                return Ok(ApiResponse<List<ProductResponse>>.ErrorResult
                    ($"No products found for category {categoryId}"));
            }
            return Ok(ApiResponse<List<ProductResponse>>.SuccessResult(pbyc));
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
