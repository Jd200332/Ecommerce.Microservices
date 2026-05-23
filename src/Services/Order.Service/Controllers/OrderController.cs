using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Order.Service.DTOs;
using Order.Service.Services;
using System.Security.Claims;

namespace Order.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder(CreateOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<OrderResponse>.ErrorResult("User not authenticated"));
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(
                    userId,
                    request.TotalAmount, // Pass actual total, not 0
                    request,
                    "Pending",
                    request.ShippingAddress
                );
                return Ok(ApiResponse<OrderResponse>.SuccessResult(order, "Order created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<OrderResponse>.ErrorResult(ex.Message));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponse>>>> GetMyOrders(
            [FromQuery] int? orderId = null,
            [FromQuery] int? productId = null,
            [FromQuery] string productName = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<IEnumerable<OrderResponse>>.ErrorResult("User not authenticated"));
            }

            try
            {
                var orders = await _orderService.GetUserOrdersAsync(
                    userId,
                    orderId,
                    productId,
                    productName
                // Removed the extra 0, 0, 0, 0 parameters
                );
                return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResult(orders));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<IEnumerable<OrderResponse>>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrderById(int id) // Renamed for clarity
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<OrderResponse>.ErrorResult("User not authenticated"));
            }

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id, userId);
                return Ok(ApiResponse<OrderResponse>.SuccessResult(order));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<OrderResponse>.ErrorResult(ex.Message));
            }
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate user is authenticated
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            try
            {
                var result = await _orderService.CancelOrderAsync(id, userId);
                return Ok(ApiResponse<bool>.SuccessResult(result, "Order cancelled"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] string status)
        {
            // Validate status is not null or empty
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Status cannot be empty"));
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, status);
                return Ok(ApiResponse<bool>.SuccessResult(result, "Status updated"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }
    }
}