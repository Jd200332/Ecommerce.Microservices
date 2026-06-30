using Order.Service.DTOs;


namespace Order.Service.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(string userId, decimal TotalAmount, 
             CreateOrderRequest request, string Status, string ShippingAddress);
        Task<IQueryable<OrderResponse>> GetUserOrdersAsync(string userId,
                int? orderId = null, 
                int? productId = null,
                string productName = null,
                decimal? minPrice = null,
                int? minQuantity = null,
                decimal? minSubTotal = null);
        Task<OrderResponse> GetOrderByIdAsync(int orderId,  string userId);
        Task<bool> UpdateOrderStatusAsync(int OrderId, string status);
        Task<bool> CancelOrderAsync(int OrderId, string userId);

    }
}
