using Cart.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Cart.Service.DTOs;

namespace Cart.Service.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            await cartService.AddItem(
                request.UserId,
                request.ProductId,
                request.Quantity,
                request.ProductName,
                request.Price
            );

            return Ok("Item added to cart");
        }
    }

    
}
