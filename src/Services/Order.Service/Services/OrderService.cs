using ECommerce.MessageBus;
using ECommerce.MessageBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.Service.Data;
using Order.Service.DTOs;
using Order.Service.Models;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace Order.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMessageBus messageBus;
        private readonly ILogger<OrderService> logger;
       

        public OrderService(
            OrderDbContext context,
            IHttpClientFactory httpClientFactory,
            IMessageBus messageBus,
            ILogger<OrderService> logger)
        {
            this.context = context;
            this.httpClientFactory = httpClientFactory;
            this.messageBus = messageBus;
            this.logger = logger;
        }


        public async Task<OrderResponse> CreateOrderAsync(string userId, decimal TotalAmount,
            CreateOrderRequest request, string Status, string ShippingAddress)
        {
         
            
        }


        public async Task<IQueryable<OrderResponse>> GetUserOrdersAsync( )
        {

        }


        public async Task<OrderResponse> GetOrderByIdAsync(int orderId, string userId)
        {
            
        }

        public async Task<bool> UpdateOrderStatusAsync(int OrderId, string status)
        {

        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {

        }
    }
}

