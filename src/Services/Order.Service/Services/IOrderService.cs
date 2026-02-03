using Order.Service.DTOs;


namespace Order.Service.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(string userId, decimal TotalAmount, 
             CreateOrderRequest request, string Status, string ShippingAddress);
        Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId,int OrderId, int ProductId, string ProductName, decimal Price, int Quantity, decimal SubTotal);
        Task<OrderResponse> GetOrderByIdAsync(int orderId,  string userId);
        Task<bool> UpdateOrderStatusAsync(int OrderId, string status);

        Task<bool> CancelOrderAsync(int OrderId, string userId);

    }
}
