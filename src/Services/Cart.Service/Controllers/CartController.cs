using Cart.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Cart.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using ECommerce.MessageBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Cart.Service.Models;
using Amazon.XRay.Recorder.Core.Sampling.Local;
using GSF.Threading;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Identity.Client;

namespace Cart.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly IAdminAccess adminaccess;
        private readonly ILogger<CartController> logger;
        

        public CartController(ICartService cartService,
            ILogger<CartController> logger,
            IAdminAccess adminacesss
           )
        {
            this.cartService = cartService;
            this.logger = logger;
            this.adminaccess = adminacesss;
            
        }

        [HttpGet("{userId}")]
        [EnableRateLimiting("read")]
        public async Task<ActionResult<List<GetCartResponse>>> GetCart(int userId, int page = 1, int pagesize = 10)
        {
            var cart = await cartService.GetCart(userId, page, pagesize);

            return Ok(cart);

        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<CartData>> Addtocart(AddToCartRequest requ, int userId)
        {
            var cart = await cartService.Addtocart(requ, userId);

            return Ok(cart);
        }


        [HttpDelete]
        public async Task<IActionResult> ClearCart(GetCartResponse response, int UserId)
        {
            await cartService.ClearCart(response, UserId);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTotal(int userId)
        {
            var total = await cartService.GetTotal(userId);

            return Ok(total);
        }


        [HttpGet("admin")]
        public async Task<IActionResult> GetCartforadmin(int page = 1, int pagesize = 10)
        {
            var data1 = await adminaccess.GetCartforadmin(page, pagesize);

            return Ok(data1);
        }

    }

    
}
