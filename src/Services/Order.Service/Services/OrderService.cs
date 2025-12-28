using ECommerce.MessageBus;
using ECommerce.MessageBus.Events;
using Microsoft.EntityFrameworkCore;
using Order.Service.Data;
using Order.Service.DTOs;
using Order.Service.Models;
using System.Diagnostics;

namespace Order.Service.Services
{
    public class OrderService
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


        public async Task<OrderResponse> CreateOrderAsync(string userId, CreateOrderRequest request)
        {
            var order = new Models.Order
            {
                UserId = userId,
                ShippingAddress = request.ShippingAddress,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                OrderItems = new List<Models.OrderItem>()
            };

            decimal totalAmount = 0;

            var productClient = httpClientFactory.CreateClient("ProductService");

            foreach(var item in request.Items)
            {
                var response = await productClient.GetAsync($"/api/product/{item.ProductId}");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Product {item.ProductId} not found");


                var productData = await response.Content.ReadFromJsonAsync<dynamic>();
                var price = (decimal)productData.data.price;



                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = productData.data.name,
                    Price = price,
                    Quantity = item.Quantity,
                    SubTotal = price * item.Quantity
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.SubTotal;
            }

            order.TotalAmount = totalAmount;
            context.Orders.Add(order);
            await context.SaveChangesAsync();


            var orderEvent = new OrderCreatedEvent
            {
                orderid = order.Id,
                userid = userId,
                totalamount = totalAmount,
                createdat = order.CreatedAt,
                items = order.OrderItems.Select(i => new OrderItemEvent
                {
                    productid = i.ProductId,
                    quantity = i.Quantity,
                    price = i.Price
                }).ToList()
            };

            await messageBus.PublishAsync(orderEvent, "order-created");

            return await GetOrderByIdAsync(order.Id, userId);
        }


        public async Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderResponse
                {
                    Id = o.Id.ToString(),
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,
                    City = o.City,
                    State = o.State,
                    Country = o.Country,
                    ZipCode = o.ZipCode,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(i => new OrderItemResponse
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        SubTotal = i.SubTotal
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<OrderResponse> GetOrderByIdAsync(int orderId, string userId)
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);


            if (order == null)
                throw new InvalidOperationException
                    ("Order not found");

            return new OrderResponse
            {
                Id = order.Id.ToString(),
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                City = order.City,
                State = order.State,
                Country = order.Country,
                ZipCode = order.ZipCode,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    SubTotal = i.SubTotal
                }).ToList()
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int OrderId, string status)
        {
            var order = await context.Orders.FindAsync(OrderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");


            order.Status = status;
            order.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            if (order.Status != "Pending")
                throw new InvalidOperationException("Cannot cancel order in current status");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }
    }
}

