using Order.Service.DTOs;


namespace Order.Service.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(string userId, CreateOrderRequest request);
        Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId);
        Task<OrderResponse> GetOrderByIdAsync(int orderId,  string userId);
        Task<bool> UpdateOrderStatusAsync(int OrderId, string status);

        Task<bool> CancelOrderAsync(int OrderId, string userId);

    }
}
