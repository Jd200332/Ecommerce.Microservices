using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Order.Service.DTOs;
using Order.Service.Services;
using System.Security.Claims;

namespace Order.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            orderService = this.orderService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder(CreateOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderService.CreateOrderAsync(userId, request);
            return Ok(ApiResponse<OrderResponse>.SuccessResult(order, "Order created successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponse>>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var orders = await orderService.GetUserOrdersAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResult(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrders(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderService.GetOrderByIdAsync(id, userId);
            return Ok(ApiResponse<OrderResponse>.SuccessResult(order));
        }


        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await orderService.CancelOrderAsync(id, userId);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Order cancelled"));
        }

        [HttpPatch("{id}/status")]
        
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] string status)
        {
            var result = await orderService.UpdateOrderStatusAsync(id, status);
            return Ok(ApiResponse<bool>.SuccessResult(result, "Status updated"));
        }

    }
}
